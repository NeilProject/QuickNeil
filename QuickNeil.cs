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
// Version: 20.07.17
// EndLic

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrickyUnits;

namespace QuickNeil {
    class QuickNeil {
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
            Launch.Hello();
        }

        static void Main(string[] args) {
            try {
                MKL.Lic    ("Quick Neil - QuickNeil.cs","GNU General Public License 3");
                MKL.Version("Quick Neil - QuickNeil.cs","20.07.17");
                if (args.Length==0) {
                    Head();
                    QCol.Cyan("Usage: ");
                    QCol.Yellow($"{qstr.StripAll(MKL.MyExe)} ");
                    QCol.Green("<script file> ");
                    QCol.Magenta("[<arguments>]\n\n");
                    QCol.White($"{MKL.All()}\n\n");
                    return;
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