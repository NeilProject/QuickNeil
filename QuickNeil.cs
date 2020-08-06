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
// Version: 20.08.06
// EndLic

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrickyUnits;
using NLua;

namespace QuickNeil {
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
            State = new Lua();
            var CallScript = $"ls = loadstring or load\nlocal LoadNeil = assert(ls(\"{NeilScript.Replace("\\","\\\\").Replace("\n", "\\n").Replace("\r", "").Replace("\"","\\\"")}\",\"Neil itself\"))\nNeil = LoadNeil()";
            Debug.WriteLine(CallScript);
            State.DoString(CallScript, "Call Neil itself");
            // newk,oftype,rw,defaultvalue
            State.DoString($"Neil.Globals('Args','table','constant',{args})","Set CLI arguments");
            TrickyDebug.Chat("Loaded Neil");
            var s = QuickStream.LoadString(Dirry.AD(script));
            TrickyDebug.Chat($"Loaded Script: {script}");
            //var scr = s.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\r", "").Replace("\"", "\\\"");
            var scr = s.Replace("\\","\\\\");
            for (int i = 0; i < 256; i++) {
                if (i < 32 || i > 120 || (char)i == '"' ) scr = scr.Replace($"{(char)i}", $"\\{qstr.Right($"00{i}", 3)}");
            }
            // Console.WriteLine($"<C#>{scr}</C#>");
            TrickyDebug.Chat("String secured");
            var sendscr = $"local translation = assert(Neil.Translate(\"{scr}\",\"Translate: {script}\"))\n\nQUICKNEILSCRIPTFUNCTION = assert(ls(translation,\"{script}\"))\n";
            Debug.WriteLine(sendscr);
            State.DoString(sendscr,"Translating");
            State.DoString("QUICKNEILSCRIPTFUNCTION()","Run-Time");
            
        }

        static void Main(string[] args) {
            try {
                MKL.Lic    ("Quick Neil - QuickNeil.cs","GNU General Public License 3");
                MKL.Version("Quick Neil - QuickNeil.cs","20.08.06");
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
                        if (i > 2) addargs.Append(", ");
                        var arg = args[i].Replace("\\","\\\\");
                        for (int j = 0; j < 256; j++) {
                            if (j < 32 || j > 120 || (char)j == '"') arg = arg.Replace($"{(char)j}", $"\\{qstr.Right($"00{j}", 3)}");
                        }
                        addargs.Append($"\"{addargs}\"");
                    }
                    addargs.Append("}");
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