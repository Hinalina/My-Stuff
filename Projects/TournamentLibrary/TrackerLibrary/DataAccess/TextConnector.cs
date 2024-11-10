using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private const string PrizesFile = "PrizeList.csv";
        private const string PeopleFile = "PersonList.cvs";
        private const string TeamFile = "TeamList.cvs";
        private const string TournamentFile = "TournamentList.cvs";
        private const string MatchupFile = "MatchupList.csv";
        private const string MatchupEntryFile = "MatchupEntryList.cvs";

        public PersonModel CreatePerson(PersonModel model)
        {
            // Load the textfile and convert the text
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // Find the max ID
            int currentId = 1;
            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;

            // Add the new record with the new ID
            people.Add(model);

            // Save the prizes to the textfile
            people.SaveToPeopleFile(PeopleFile);

            return model;
        }

        public PrizeModel CreatePrize(PrizeModel model)
        {
            // Load the existing list of prizes from the file and convert them into PrizeModel information
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // Determine the next ID for the new prize
            int currentId = 1;
            if (prizes.Count > 0)
            {
                // Get the highest ID from the existing prizes and add 1 for the new prize's ID
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }

            // Assign the new prize the next available ID
            model.Id = currentId;

            // Add the new prize to the list of prizes
            prizes.Add(model);

            // Save the updated list of prizes back to the file
            prizes.SaveToPrizeFile(PrizesFile);

            // Return the created PrizeModel (including the new ID)
            return model;
        }

        public List<PersonModel> GetPerson_All()
        {
            // Load the data from the file and convert it into a list of PersonModel
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }
        public TeamModel CreateTeam(TeamModel model)
        {
            // Load the existing list of teams from the file and convert them into TeamModel
            // Also converts associated people data using PeopleFile
            List<TeamModel> teams = TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            // Determine the next available ID for the new team
            int currentId = 1;
            if (teams.Count > 0)
            {
                // Get the highest ID from the existing teams and add 1 for the new team's ID
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }

            // Assign the new team the next available ID
            model.Id = currentId;

            // Add the newly created team to the list of teams
            teams.Add(model);

            // Save the updated list of teams back to the team file
            teams.SaveToTeamFile(TeamFile);

            // Return the newly created team (including the assigned ID)
            return model;
        }

        public List<TeamModel> GetTeam_All()
        {
            // Load the data from the team file, convert it into a list of TeamModel,
            // and also convert associated people data using PeopleFile
            return TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }

        public void CreateTournament(TournamentModel model)
        {
            // Load the existing list of tournaments from the file and convert them into TournamentModel
            // It also converts the associated team, person, and prize data
            List<TournamentModel> tournamens = TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);

            // Determine the next available ID for the new tournament
            int currentId = 1;
            if (tournamens.Count > 0)
            {
                // Get the highest ID from the existing tournaments and add 1 for the new tournament's ID
                currentId = tournamens.OrderByDescending(x => x.Id).First().Id + 1;
            }

            // Assign the new tournament the next available ID
            model.Id = currentId;

            // Save the rounds of the tournament to the file, including matchups and matchup entries
            model.SaveRoundsToFile(MatchupFile, MatchupEntryFile);

            // Add the new tournament to the list of tournaments
            tournamens.Add(model);

            // Save the updated list of tournaments back to the tournament file
            tournamens.SaveToTournamentFile(TournamentFile);
        }

        public List<TournamentModel> GetTournament_All()
        {
            return TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
