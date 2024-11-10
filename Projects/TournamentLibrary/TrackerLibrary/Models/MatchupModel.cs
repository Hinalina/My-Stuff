using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class MatchupModel
    {
        public int Id { get; set; }
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        public int WinnerId { get; set; }
        public TeamModel Winner { get; set; }
        public int MatchupRound { get; set; }

        // Get a string representation of the matchup's display name
        public string DisplayName
        {
            get
            {
                // Initialize an empty string to store the output
                string output = "";

                // Loop through each MatchupEntryModel in the Entries list
                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        // Check if the output string is empty (first entry in the matchup)
                        if (output.Length == 0)
                        {
                            // Set the output to the team name of the first matchup entry
                            output = me.TeamCompeting.TeamName;
                        }
                        else
                        {
                            // If output is not empty, append the next team's name with " vs." separator
                            output += $" vs. {me.TeamCompeting.TeamName}";
                        }
                    }
                    else
                    {
                        output = "Matchup Not Yet Determined";
                        break;
                    }
                }

                // Return the final output string which shows the matchup teams
                return output;
            }
        }
    }
}
