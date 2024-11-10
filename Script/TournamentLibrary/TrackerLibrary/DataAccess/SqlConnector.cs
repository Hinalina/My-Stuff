using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        // Not finished since the Sql Connection doesn't work
        public PersonModel CreatePerson(PersonModel model)
        {
            throw new NotImplementedException();
        }

        // Not finished since the Sql Connection doesn't work
        public List<PersonModel> GetPerson_All()
        {
            throw new NotImplementedException();
        }

        // Not finished since the Sql Connection doesn't work
        public TeamModel CreateTeam(TeamModel model)
        {
            throw new NotImplementedException();
        }

        // Not finished since the Sql Connection doesn't work
        public List<TeamModel> GetTeam_All()
        {
            throw new NotImplementedException();
        }

        // Not finished since the Sql Connection doesn't work
        public void CreateTournament(TournamentModel model)
        {
            throw new NotImplementedException();
        }

        // Not finished since the Sql Connection doesn't work
        public List<TournamentModel> GetTournament_All()
        {
            throw new NotImplementedException();
        }
        public void UpdateMatchup(MatchupModel model)
        {
            throw new NotImplementedException();
        }

        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(GlobalConfig.CnnString("Tournaments")))
            {
                // 6:08:04

                var p = new DynamicParameters();

                p.Add("@PlaceNumber", model.PlaceNumber);
                p.Add("@PlaceName", model.PlaceName);
                p.Add("@PrizeAmount", model.PrizeAmount);
                p.Add("@PrizePercentage", model.PrizePrecentage);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                return model;
            }
        }

    }
}
