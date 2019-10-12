using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public class Replay
    {
        private static int replaySession = 1;
        StreamWriter replayWriter = null;
        public void StartRecordingReplay()
        {
            Process p = Process.GetCurrentProcess();
            var fileName = "si_" + p.Id + "_" + p.StartTime.ToString("ddMMMyyyy_hhmm") + "_" + replaySession + ".txt";
            replayWriter = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), fileName));
            replayWriter.AutoFlush = true;
            replaySession += 1;
        }
        public void CompleteReplay()
        {
            if (replayWriter != null)
            {
                replayWriter.Close();
            }
        }
        public void recordCommands(long loopId, GameInput input)
        {
            if(replayWriter != null)
            {
                var keys = input.Keys;

                if(keys.Length > 0)
                {
                    string line = string.Concat(loopId, ",", string.Join("|", keys));
                    replayWriter.WriteLine(line);
                }
            }
        }
        public static Queue<ReplayEvent> Load(string filename)
        {
            //TODO: Determine how to handle file issues (non existent, corrupt) and show the errors appropriately from back to front
            var list = new Queue<ReplayEvent>();
            using(StreamReader sr = new StreamReader(filename))
            {
                string s = "";
                while((s = sr.ReadLine()) != null)
                {
                    list.Enqueue(ReplayEvent.GetEvent(s));
                }
            }
            
            return list;
        }
    }
}
