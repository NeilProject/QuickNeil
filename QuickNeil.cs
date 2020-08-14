// Lic:
// Quick Neil
// Main Portion
// 
// 
// 
// (c) Jeroen P. Broks, 2020
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
// Please note that some references to data like pictures or audio, do not automatically
// fall under this licenses. Mostly this is noted in the respective files.
// 
// Version: 20.08.14
// EndLic

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using TrickyUnits;
using NLua;

namespace QuickNeil {

    class API {
        Lua State;
        string Script;
        public API(Lua State, string Script) {
            this.State = State;
            this.Script = Script;
            this.State["LuaQuickNeil"] = this;
            this.State.DoString($"Neil.Load([[{QuickStream.StringFromEmbed("QuickNeil.Neil")}]],\"QuickNeil API Linkup\")()", "QuickNeil API Linkup from C#");
        }

        public string ScriptVersion() {
            var ss = QuickStream.LoadString(Script);
            var s = ss.Split('\n');
            foreach (string ln in s) {
                //Console.WriteLine($"TEST:  {ln.Trim().ToUpper()} >> {qstr.Prefixed(ln.Trim().ToUpper(), "#MKL_VERSION")}");
                if (qstr.Prefixed(ln.Trim().ToUpper(), "#MKL_VERSION")) {
                    var version = new StringBuilder("");
                    var quotes = 0;
                    for (int i = 0; i < ln.Length; i++) {
                        if (ln[i] == '"') quotes++;
                        else if (quotes == 3) version.Append($"{ln[i]}");
                    }
                    return $"{version}";
                }
            }
            return "<< Script Version Not Retreived >>";
        }

        public string GetDir(string path, bool hidden) {
            var lijst = FileList.GetDir(Dirry.AD(path), 0, true, hidden);
            return mkLuaStringFromTable(lijst);
        }

        public string GetTree(string path, bool hidden) {
            var lijst = FileList.GetTree(Dirry.AD(path), true, hidden);
            return mkLuaStringFromTable(lijst);
        }

        public bool FileExists(string file) => File.Exists(file);
        public bool DirExists(string file) => Directory.Exists(file);
        public string MD5(string str) => qstr.md5(str);

        public string LoadString(string file) {
            if (!FileExists(file)) return "";
            return QuickStream.LoadString(file);
        }

        public void SaveString(string file, string Content) {
            QuickStream.SaveString(file, Content);
        }


        string mkLuaStringFromTable(string[] l) {
            var ret = new StringBuilder("return {");
            for (int i = 0; i < l.Length; ++i) {
                if (i > 0) ret.Append(", ");
                ret.Append($"\"{l[i]}\"");
            }
            ret.Append("}");
            return $"{ret}";
        }

        public string FileInfo(string f) {
            try {
                var fi = new FileInfo(f);
                return $"return {"{"} Size={fi.Length}, Created='{fi.CreationTime}', Modified='{fi.LastWriteTime}', Attrib='{fi.Attributes}' {"}"}";
            } catch (Exception E) {
                QCol.QuickError($".NET exception: '{E.Message}' while accessing {f}");
                Environment.Exit(100);
                // State.DoString($"error([[{E.Message}\n\n.NET ERROR\n\n]]..debug.traceback)", "Error in .NET");
                return "return {Size=0, Created='Error', Modified='Error', Attrib='Error'}";
            }
        }
    }

    class QuickNeil {

        static Lua State;
        static readonly string NeilScript = QuickStream.StringFromEmbed("Neil.lua");

        static void Head() {
            QCol.Yellow("Quick Neil ");
            QCol.Cyan($"{MKL.Newest}\n");
            QCol.Green($"(c) Jeroen P. Broks {MKL.CYear(2020)}\n");
            QCol.Magenta("Released under the terms of the GPL3\n\n");
        }

        static QuickNeil() {
            QCol.Hello();
            Dirry.InitAltDrives();
            FileList.Hello();
            qstr.Hello();
            QuickStream.Hello();
            //Launch.Hello();
        }


        static void LoadScript(string script,string args) {
            script = Dirry.AD(script.Replace("\\", "/"));
            State = new Lua();
            var CallScript = $"ls = loadstring or load\nlocal LoadNeil = assert(ls(\"{NeilScript.Replace("\\","\\\\").Replace("\n", "\\n").Replace("\r", "").Replace("\"","\\\"")}\",\"Neil itself\"))\nNeil = LoadNeil()";
            Debug.WriteLine(CallScript);
            State.DoString(CallScript, "Call Neil itself");
            // newk,oftype,rw,defaultvalue
            // if (true) throw new Exception(args);
            State.DoString($"Neil.Globals('Args','table','constant',{args})","Set CLI arguments");
            new API(State, script);
            TrickyDebug.Chat("Loaded Neil");
            var s = QuickStream.LoadString(Dirry.AD(script));
            TrickyDebug.Chat($"Loaded Script: {script}");
            //var scr = s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "").Replace("\"", "\\\"");
            var scr = s.Replace("\\","\\\\");
            for (int i = 0; i < 256; i++) {
                if (i < 32 || i > 120 || (char)i == '"' ) scr = scr.Replace($"{(char)i}", $"\\{qstr.Right($"00{i}", 3)}");
            }
            // Console.WriteLine($"<C#>{scr}</C#>");
            // Console.WriteLine($"Script: {script}");
            TrickyDebug.Chat("String secured");
            var sendscr = $"local translation = assert(Neil.Translate(\"{scr}\",\"Translate: {script}\"))\n\nlocal err; QUICKNEILSCRIPTFUNCTION,err = ls(translation,\"{script}\")\n\nif not QUICKNEILSCRIPTFUNCTION then error(\"Compiling the translation failed\\n\"..tostring(err)..\"\\n\\n\"..translation) end";
            //Console.WriteLine($"<TRANSLATION>\n{sendscr}\n</TRANSLATION>");
            Debug.WriteLine(sendscr);
            State.DoString(sendscr,"Translating");
            State.DoString("QUICKNEILSCRIPTFUNCTION()","Run-Time");
            
        }

        static void Main(string[] args) {
            try {
                MKL.Lic    ("Quick Neil - QuickNeil.cs","GNU General Public License 3");
                MKL.Version("Quick Neil - QuickNeil.cs","20.08.14");
                if (args.Length==0) {
                    Head();
                    QCol.Cyan("Usage: ");
                    QCol.Yellow($"{qstr.StripAll(MKL.MyExe)} ");
                    QCol.Green("<script file> ");
                    QCol.Magenta("[<arguments>]\n\n");
                    QCol.White($"{MKL.All()}\n\n");
                    return;
                } else {
                    var addargs = new StringBuilder("{");
                    for(int i=1;i<args.Length;i++) {
                        if (i >= 2) addargs.Append(", ");
                        var arg = args[i].Replace("\\","\\\\");
                        for (int j = 0; j < 256; j++) {
                            if (j < 32 || j > 120 || (char)j == '"') arg = arg.Replace($"{(char)j}", $"\\{qstr.Right($"00{j}", 3)}");
                        }
                        addargs.Append($"\"{arg}\"");
                    }
                    addargs.Append("}");
                    Debug.WriteLine($"DBG:Argument array {addargs}");
                    //throw new Exception ($"Argument array {addargs}"); // debug only!
                    LoadScript(args[0],$"{addargs}");
                }
            } catch (Exception e) {
                QCol.QuickError(e.Message);
#if DEBUG
                QCol.Cyan("Traceback\n");
                QCol.White($"{e.StackTrace}\n");
#endif
            } finally {
                TrickyDebug.AttachWait();
            }
        }
    }
}