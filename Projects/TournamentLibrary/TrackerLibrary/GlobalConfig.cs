using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public const string PrizesFile = "PrizeList.csv";
        public const string PeopleFile = "PersonList.cvs";
        public const string TeamFile = "TeamList.cvs";
        public const string TournamentFile = "TournamentList.cvs";
        public const string MatchupFile = "MatchupList.csv";
        public const string MatchupEntryFile = "MatchupEntryList.cvs";

        public static IDataConnection Connection { get; private set; }

        public static void InitializeConnections(DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                // TODO - Set up the SQL Connector properly
                SqlConnector sql = new SqlConnector();
                Connection = (sql);
            }
            else if (db == DatabaseType.Textfile)
            {
                // TODO - Create the Text Connections
                TextConnector text = new TextConnector();
                Connection = (text);
            }
        }

        public static string CnnString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
