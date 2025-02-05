using System;

namespace EasySaveConsole.EasySaveNamespace.State
{
    public class StateEntry
    {
        public string JobName { get; set; }           
        public string Timestamp { get; set; }       
        public string Status { get; set; }           
        public int FilesTotal { get; set; }          
        public int SizeTotal { get; set; }           
        public float Progress { get; set; }          
        public int RemainingFiles { get; set; }      
        public int RemainingSize { get; set; }       
        public string CurrentSource { get; set; }    
        public string CurrentTarget { get; set; }  

        public StateEntry()
        {
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Status = "Non Actif"; // Par défaut, le job est inactif
        }
    }
}
