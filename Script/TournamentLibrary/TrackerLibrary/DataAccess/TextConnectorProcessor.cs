using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName)
        {
            return $"{ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
        }

        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            // Create a list to hold the PrizeModel information
            List<PrizeModel> output = new List<PrizeModel>();

            // Loop through each line in the input list
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);                    // Parse and set the prize ID
                p.PlaceNumber = int.Parse(cols[1]);           // Parse and set the place number
                p.PlaceName = cols[2];                        // Set the place name
                p.PrizeAmount = decimal.Parse(cols[3]);       // Parse and set the prize amount
                p.PrizePrecentage = double.Parse(cols[4]);    // Parse and set the prize percentage

                output.Add(p);
            }

            return output;
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            // Create a list to hold the PersonModel information
            List<PersonModel> output = new List<PersonModel>();

            // Loop through each line in the input list
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);           // Parse and set the person ID
                p.FirstName = cols[1];               // Set the first name
                p.LastName = cols[2];                // Set the last name
                p.EmailAdress = cols[3];             // Set the email address
                p.CellphoneNumber = cols[4];         // Set the cellphone number

                output.Add(p);
            }

            return output;
        }
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            // Create a list to hold the TeamModel information
            List<TeamModel> output = new List<TeamModel>();

            // Load the list of PersonModel objects from the specified file
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            // Loop through each line in the input list
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel t = new TeamModel();
                t.Id = int.Parse(cols[0]);         // Parse and set the team ID
                t.TeamName = cols[1];              // Set the team name

                // Split the team members' IDs by '|' and find each person by ID
                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    // Find the person with the matching ID and add to TeamMembers list
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }

                output.Add(t);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines, string teamFileName, string peopleFileName, string prizeFileName)
        {
            // Create a list to hold the TournamentModel information
            List<TournamentModel> output = new List<TournamentModel>();

            // Load the list of TeamModel, PrizeModel, and MatchupModel information from their respective files
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            // Loop through each line in the input list
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel();
                tm.Id = int.Parse(cols[0]);                    // Parse and set the tournament ID
                tm.TournamentName = cols[1];                   // Set the tournament name
                tm.EntryFee = decimal.Parse(cols[2]);          // Parse and set the entry fee

                string[] teamIds = cols[3].Split('|');

                // Ensure EnteredTeams is initialized before adding teams
                if (tm.EnteredTeams == null)
                {
                    tm.EnteredTeams = new List<TeamModel>();  // Initialize the list if it's null
                }

                foreach (string id in teamIds)
                {
                    // Find the team by ID using FirstOrDefault (which returns null if not found)
                    TeamModel team = teams.FirstOrDefault(x => x.Id == int.Parse(id));

                    // Ensure that the team was found before adding it to the list
                    if (team != null)
                    {
                        tm.EnteredTeams.Add(team);
                    }
                }

                // Check if (Prizes) has a non-empty value
                if (cols[4].Length > 0)
                {
                    string[] prizeIds = cols[4].Split('|');

                    // Loop through each prize id
                    foreach (string id in prizeIds)
                    {
                        // Find the prize from the list of prizes where the Id matches the current prize id
                        // Add the matched prize to the tournament's Prizes list
                        tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                    }
                }

                string[] rounds = cols[5].Split('|');

                // Convert rounds (collections of matchups) to lists of MatchupModel
                foreach (string round in rounds)
                {
                    // Split each round by '^' to get the individual matchup IDs
                    string[] msText = round.Split('^');
                    List<MatchupModel> ms = new List<MatchupModel>();

                    // Convert matchup IDs to MatchupModel and add to the round
                    foreach (string matchupModelTextId in msText)
                    {
                        ms.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
                    }

                    // Add the round (list of matchups) to the Rounds list in TournamentModel
                    tm.Rounds.Add(ms);
                }

                output.Add(tm);
            }

            return output;
        }

        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            // Create a list of strings to store each prize's data in a formatted way
            List<string> lines = new List<string>();

            // Loop through each PrizeModel in the input list
            foreach (PrizeModel p in models)
            {
                // Convert each PrizeModel's properties to a comma-separated string and add to lines
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePrecentage}");
            }

            // Write all formatted lines to the specified file
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            // Create a list of strings to store each person's data in a formatted way
            List<string> lines = new List<string>();

            // Loop through each PersonModel in the input list
            foreach (PersonModel p in models)
            {
                // Convert each PersonModel's properties to a comma-separated string and add to lines
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAdress},{p.CellphoneNumber}");
            }

            // Write all formatted lines to the specified file
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            // Create a list of strings to store each team's data in a formatted way
            List<string> lines = new List<string>();

            // Loop through each TeamModel object in the input list
            foreach (TeamModel t in models)
            {
                // Convert each TeamModel's properties to a comma-separated string and add to lines
                lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");
            }

            // Write all formatted lines to the specified file
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveRoundsToFile(this TournamentModel model, string matchupFile, string matchupEntryFile)
        {
            // Loop through each round (list of matchups) in the tournament
            foreach (List<MatchupModel> round in model.Rounds)
            {
                // Loop through each matchup in the current round
                foreach (MatchupModel matchup in round)
                {
                    // Save the matchup to the specified files
                    matchup.SaveMatchupToFile(matchupFile, matchupEntryFile);
                }
            }
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            // Create a list to hold the MatchupEntryModel information
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            // Loop through each line in the input list
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupEntryModel me = new MatchupEntryModel();

                me.Id = int.Parse(cols[0]);   // Parse and set the entry ID

                // Check if TeamCompeting is null or needs to be set
                if (cols[1].Length == 0)
                {
                    me.TeamCompeting = null;    // No team competing in this entry
                }
                else
                {
                    me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));   // Lookup and set the competing team
                }
                me.Score = double.Parse(cols[2]);     // Parse and set the score

                // Check if there's a valid ParentMatchup ID and set ParentMatchup accordingly
                int parentId = 0;
                if (int.TryParse(cols[3], out parentId))
                {
                    me.ParentMatchup = LookupMatchupById(int.Parse(cols[3]));     // Lookup and set the parent matchup
                }
                else
                {
                    me.ParentMatchup = null;    // No parent matchup for this entry
                }

                output.Add(me);
            }

            return output;
        }

        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<MatchupEntryModel>();
            }

            // Split the input string by '|' to get individual matchup entry IDs
            string[] ids = input.Split('|');

            // Create a list to store the converted MatchupEntryModel information
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            // Load the list of matchup entries from the file
            List<string> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile();

            // Initialize a list to store entries that match the provided ids
            List<string> matchingEntries = new List<string>();

            // Loop through each id provided
            foreach (string id in ids)
            {
                // Loop through each entry in the loaded entries
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    // Check if the first column (Id) matches the current id
                    if (cols[0] == id)
                    {
                        // If it matches, add the entry to the matchingEntries list
                        matchingEntries.Add(entry);
                    }
                }
            }

            // Convert the matching entries to MatchupEntryModel
            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        private static TeamModel LookupTeamById(int id)
        {
            // Load the list of teams from the team file
            List<string> teams = GlobalConfig.TeamFile.FullFilePath().LoadFile();

            // Loop through each team entry in the loaded teams list
            foreach (string team in teams)
            {
                string[] cols = team.Split(',');

                // Check if the first column (Id) matches the provided id
                if (cols[0] == id.ToString())
                {
                    // Initialize a list to store matching teams
                    List<string> matchingTeams = new List<string>();

                    // Add the current matching team to the list
                    matchingTeams.Add(team);

                    // Convert the matching team to a TeamModel using the ConvertToTeamModels method,
                    // passing in the PeopleFile for related person data
                    // Return the first matching team (assuming id uniqueness)
                    return matchingTeams.ConvertToTeamModels(GlobalConfig.PeopleFile).First();
                }
            }

            return null;
        }

        public static MatchupModel LookupMatchupById(int id)
        {
            // Load the list of matchups from the matchup file
            List<string> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile();

            // Loop through each matchup entry in the loaded matchups list
            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');

                // Check if the first column (Id) matches the provided id
                if (cols[0] == id.ToString())
                {
                    // Initialize a list to store matching matchups
                    List<string> matchingMatchups = new List<string>();

                    // Add the current matching matchup to the list
                    matchingMatchups.Add(matchup);

                    // Convert the matching matchup to a MatchupModel using the ConvertToMatchupModels method
                    // Return the first matching matchup (assuming id uniqueness)
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            return null;
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            // Create a list to store the converted MatchupModel information
            List<MatchupModel> output = new List<MatchupModel>();

            // Loop through each line in the input list
            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                MatchupModel m = new MatchupModel();
                m.Id = int.Parse(cols[0]);                                    // Parse and set the ID of the matchup
                m.Entries = ConvertStringToMatchupEntryModels(cols[1]);       // Convert the string of matchup entry IDs into a list of MatchupEntryModel
                if (cols[2].Length == 0)
                {
                    m.Winner = null;
                }
                else
                {
                    m.Winner = LookupTeamById(int.Parse(cols[2]));            // Lookup and set the winner of the matchup by ID
                }
                m.MatchupRound = int.Parse(cols[3]);                          // Parse and set the round of the matchup
                output.Add(m);
            }

            return output;
        }

        public static void UpdateMatchupToFile(this MatchupModel matchup)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            MatchupModel oldMatchup = new MatchupModel();

            foreach (MatchupModel m in matchups)
            {
                if (m.Id == matchup.Id)
                {
                    oldMatchup = m;
                }
            }

            matchups.Remove(oldMatchup);

            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.UpdateEntryToFile();
            }

            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }

            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        public static void SaveMatchupToFile(this MatchupModel matchup, string matchupFile, string matchupEntryFile)
        {
            // Load existing matchups from file and convert them into MatchupModel
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.FullFilePath().LoadFile().ConvertToMatchupModels();

            // Determine a new unique ID for the matchup by incrementing the highest existing ID
            int currentId = 1;
            if (matchups.Count > 0)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }
            matchup.Id = currentId; // Assign the new ID to the current matchup

            // Add the new matchup to the list of matchups
            matchups.Add(matchup);

            // Save each matchup entry to a separate file
            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                entry.SaveEntryToFile(matchupEntryFile);
            }

            // Reset the lines list to prepare for writing again with updated data
            List<string> lines = new List<string>();

            foreach (MatchupModel m in matchups)
            {
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{m.Id},{ConvertMatchupEntryListToString(m.Entries)},{winner},{m.MatchupRound}");
            }

            // Overwrite the matchup file with the updated lines
            File.WriteAllLines(GlobalConfig.MatchupFile.FullFilePath(), lines);
        }

        public static void SaveEntryToFile(this MatchupEntryModel entry, string matchupEntryFile)
        {
            // Load the existing matchup entries from the file and convert them to MatchupEntryModel
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            // Determine the current ID for the new entry (increment by 1 from the highest existing ID)
            int currentId = 1;
            if (entries.Count > 0)
            {
                // Get the highest ID and add 1 for the new entry
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }
            // Set the new entry's ID
            entry.Id = currentId;

            // Add the new entry to the list of entries
            entries.Add(entry);

            // Create a list to store the string representations of all entries for saving
            List<string> lines = new List<string>();

            // Loop through each MatchupEntryModel and create a string representation of its data
            foreach (MatchupEntryModel me in entries)
            {
                // If there's a parent matchup, get its ID; otherwise, leave it empty
                string parent = "";
                if (me.ParentMatchup != null)
                {
                    parent = me.ParentMatchup.Id.ToString();
                }

                // If there's a team competing, get its ID; otherwise, leave it empty
                string teamCompeting = "";
                if (me.TeamCompeting != null)
                {
                    teamCompeting = me.TeamCompeting.Id.ToString();
                }
                lines.Add($"{me.Id},{teamCompeting},{me.Score},{parent}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }
        public static void UpdateEntryToFile(this MatchupEntryModel entry)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            MatchupEntryModel oldEntry = new MatchupEntryModel();

            foreach(MatchupEntryModel me in entries)
            {
                if(me.Id == entry.Id)
                {
                    oldEntry = me;
                }
            }

            entries.Remove(oldEntry);

            entries.Add(entry);

            List<string> lines = new List<string>();

            foreach (MatchupEntryModel me in entries)
            {
                string parent = "";
                if (me.ParentMatchup != null)
                {
                    parent = me.ParentMatchup.Id.ToString();
                }

                string teamCompeting = "";
                if (me.TeamCompeting != null)
                {
                    teamCompeting = me.TeamCompeting.Id.ToString();
                }
                lines.Add($"{me.Id},{teamCompeting},{me.Score},{parent}");
            }

            File.WriteAllLines(GlobalConfig.MatchupEntryFile.FullFilePath(), lines);
        }

        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            // Create a list to store the string representations of each tournament
            List<string> lines = new List<string>();

            // Loop through each TournamentModel in the input list
            foreach (TournamentModel tm in models)
            {
                lines.Add($"{tm.Id},{tm.TournamentName},{tm.EntryFee},{ConvertTeamListToString(tm.EnteredTeams)},{ConvertPrizeListToString(tm.Prizes)},{ConvertRoundListToString(tm.Rounds)}");
            }

            // Write all the formatted tournament data to the file
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            // Initialize an empty string to hold the final output
            string output = "";

            // If the rounds list is empty, return an empty string
            if (rounds.Count == 0)
            {
                return "";
            }

            // Loop through each round in the rounds list
            foreach (List<MatchupModel> r in rounds)
            {
                // Convert the list of matchups in the current round to a string and append it to the output string
                // Separate rounds with a '|'
                output += $"{ConvertMatchupListToString(r)}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
            {
                return "";
            }

            // Example: 1|2|3|
            foreach (MatchupModel m in matchups)
            {
                output += $"{m.Id}^";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";

            if (entries.Count == 0)
            {
                return "";
            }

            // Example: 1|2|3|
            foreach (MatchupEntryModel me in entries)
            {
                output += $"{me.Id}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }

            // Example: 1|2|3|
            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }

            // Example: 1|2|3|
            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }

        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }

            // Example: 1|2|3|
            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }

            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}
