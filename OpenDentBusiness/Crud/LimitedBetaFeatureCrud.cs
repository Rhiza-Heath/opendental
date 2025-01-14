//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OpenDentBusiness.Crud{
	public class LimitedBetaFeatureCrud {
		///<summary>Gets one LimitedBetaFeature object from the database using the primary key.  Returns null if not found.</summary>
		public static LimitedBetaFeature SelectOne(long limitedBetaFeatureNum) {
			string command="SELECT * FROM limitedbetafeature "
				+"WHERE LimitedBetaFeatureNum = "+POut.Long(limitedBetaFeatureNum);
			List<LimitedBetaFeature> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one LimitedBetaFeature object from the database using a query.</summary>
		public static LimitedBetaFeature SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<LimitedBetaFeature> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of LimitedBetaFeature objects from the database using a query.</summary>
		public static List<LimitedBetaFeature> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<LimitedBetaFeature> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<LimitedBetaFeature> TableToList(DataTable table) {
			List<LimitedBetaFeature> retVal=new List<LimitedBetaFeature>();
			LimitedBetaFeature limitedBetaFeature;
			foreach(DataRow row in table.Rows) {
				limitedBetaFeature=new LimitedBetaFeature();
				limitedBetaFeature.LimitedBetaFeatureNum    = PIn.Long  (row["LimitedBetaFeatureNum"].ToString());
				limitedBetaFeature.LimitedBetaFeatureTypeNum= PIn.Long  (row["LimitedBetaFeatureTypeNum"].ToString());
				limitedBetaFeature.ClinicNum                = PIn.Long  (row["ClinicNum"].ToString());
				limitedBetaFeature.IsSignedUp               = PIn.Bool  (row["IsSignedUp"].ToString());
				retVal.Add(limitedBetaFeature);
			}
			return retVal;
		}

		///<summary>Converts a list of LimitedBetaFeature into a DataTable.</summary>
		public static DataTable ListToTable(List<LimitedBetaFeature> listLimitedBetaFeatures,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="LimitedBetaFeature";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("LimitedBetaFeatureNum");
			table.Columns.Add("LimitedBetaFeatureTypeNum");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("IsSignedUp");
			foreach(LimitedBetaFeature limitedBetaFeature in listLimitedBetaFeatures) {
				table.Rows.Add(new object[] {
					POut.Long  (limitedBetaFeature.LimitedBetaFeatureNum),
					POut.Long  (limitedBetaFeature.LimitedBetaFeatureTypeNum),
					POut.Long  (limitedBetaFeature.ClinicNum),
					POut.Bool  (limitedBetaFeature.IsSignedUp),
				});
			}
			return table;
		}

		///<summary>Inserts one LimitedBetaFeature into the database.  Returns the new priKey.</summary>
		public static long Insert(LimitedBetaFeature limitedBetaFeature) {
			return Insert(limitedBetaFeature,false);
		}

		///<summary>Inserts one LimitedBetaFeature into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(LimitedBetaFeature limitedBetaFeature,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				limitedBetaFeature.LimitedBetaFeatureNum=ReplicationServers.GetKey("limitedbetafeature","LimitedBetaFeatureNum");
			}
			string command="INSERT INTO limitedbetafeature (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="LimitedBetaFeatureNum,";
			}
			command+="LimitedBetaFeatureTypeNum,ClinicNum,IsSignedUp) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(limitedBetaFeature.LimitedBetaFeatureNum)+",";
			}
			command+=
				     POut.Long  (limitedBetaFeature.LimitedBetaFeatureTypeNum)+","
				+    POut.Long  (limitedBetaFeature.ClinicNum)+","
				+    POut.Bool  (limitedBetaFeature.IsSignedUp)+")";
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command);
			}
			else {
				limitedBetaFeature.LimitedBetaFeatureNum=Db.NonQ(command,true,"LimitedBetaFeatureNum","limitedBetaFeature");
			}
			return limitedBetaFeature.LimitedBetaFeatureNum;
		}

		///<summary>Inserts many LimitedBetaFeatures into the database.</summary>
		public static void InsertMany(List<LimitedBetaFeature> listLimitedBetaFeatures) {
			InsertMany(listLimitedBetaFeatures,false);
		}

		///<summary>Inserts many LimitedBetaFeatures into the database.  Provides option to use the existing priKey.</summary>
		public static void InsertMany(List<LimitedBetaFeature> listLimitedBetaFeatures,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				foreach(LimitedBetaFeature limitedBetaFeature in listLimitedBetaFeatures) {
					Insert(limitedBetaFeature);
				}
			}
			else {
				StringBuilder sbCommands=null;
				int index=0;
				int countRows=0;
				while(index < listLimitedBetaFeatures.Count) {
					LimitedBetaFeature limitedBetaFeature=listLimitedBetaFeatures[index];
					StringBuilder sbRow=new StringBuilder("(");
					bool hasComma=false;
					if(sbCommands==null) {
						sbCommands=new StringBuilder();
						sbCommands.Append("INSERT INTO limitedbetafeature (");
						if(useExistingPK) {
							sbCommands.Append("LimitedBetaFeatureNum,");
						}
						sbCommands.Append("LimitedBetaFeatureTypeNum,ClinicNum,IsSignedUp) VALUES ");
						countRows=0;
					}
					else {
						hasComma=true;
					}
					if(useExistingPK) {
						sbRow.Append(POut.Long(limitedBetaFeature.LimitedBetaFeatureNum)); sbRow.Append(",");
					}
					sbRow.Append(POut.Long(limitedBetaFeature.LimitedBetaFeatureTypeNum)); sbRow.Append(",");
					sbRow.Append(POut.Long(limitedBetaFeature.ClinicNum)); sbRow.Append(",");
					sbRow.Append(POut.Bool(limitedBetaFeature.IsSignedUp)); sbRow.Append(")");
					if(sbCommands.Length+sbRow.Length+1 > TableBase.MaxAllowedPacketCount && countRows > 0) {
						Db.NonQ(sbCommands.ToString());
						sbCommands=null;
					}
					else {
						if(hasComma) {
							sbCommands.Append(",");
						}
						sbCommands.Append(sbRow.ToString());
						countRows++;
						if(index==listLimitedBetaFeatures.Count-1) {
							Db.NonQ(sbCommands.ToString());
						}
						index++;
					}
				}
			}
		}

		///<summary>Inserts one LimitedBetaFeature into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(LimitedBetaFeature limitedBetaFeature) {
			return InsertNoCache(limitedBetaFeature,false);
		}

		///<summary>Inserts one LimitedBetaFeature into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(LimitedBetaFeature limitedBetaFeature,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO limitedbetafeature (";
			if(!useExistingPK && isRandomKeys) {
				limitedBetaFeature.LimitedBetaFeatureNum=ReplicationServers.GetKeyNoCache("limitedbetafeature","LimitedBetaFeatureNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="LimitedBetaFeatureNum,";
			}
			command+="LimitedBetaFeatureTypeNum,ClinicNum,IsSignedUp) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(limitedBetaFeature.LimitedBetaFeatureNum)+",";
			}
			command+=
				     POut.Long  (limitedBetaFeature.LimitedBetaFeatureTypeNum)+","
				+    POut.Long  (limitedBetaFeature.ClinicNum)+","
				+    POut.Bool  (limitedBetaFeature.IsSignedUp)+")";
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command);
			}
			else {
				limitedBetaFeature.LimitedBetaFeatureNum=Db.NonQ(command,true,"LimitedBetaFeatureNum","limitedBetaFeature");
			}
			return limitedBetaFeature.LimitedBetaFeatureNum;
		}

		///<summary>Updates one LimitedBetaFeature in the database.</summary>
		public static void Update(LimitedBetaFeature limitedBetaFeature) {
			string command="UPDATE limitedbetafeature SET "
				+"LimitedBetaFeatureTypeNum=  "+POut.Long  (limitedBetaFeature.LimitedBetaFeatureTypeNum)+", "
				+"ClinicNum                =  "+POut.Long  (limitedBetaFeature.ClinicNum)+", "
				+"IsSignedUp               =  "+POut.Bool  (limitedBetaFeature.IsSignedUp)+" "
				+"WHERE LimitedBetaFeatureNum = "+POut.Long(limitedBetaFeature.LimitedBetaFeatureNum);
			Db.NonQ(command);
		}

		///<summary>Updates one LimitedBetaFeature in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(LimitedBetaFeature limitedBetaFeature,LimitedBetaFeature oldLimitedBetaFeature) {
			string command="";
			if(limitedBetaFeature.LimitedBetaFeatureTypeNum != oldLimitedBetaFeature.LimitedBetaFeatureTypeNum) {
				if(command!="") { command+=",";}
				command+="LimitedBetaFeatureTypeNum = "+POut.Long(limitedBetaFeature.LimitedBetaFeatureTypeNum)+"";
			}
			if(limitedBetaFeature.ClinicNum != oldLimitedBetaFeature.ClinicNum) {
				if(command!="") { command+=",";}
				command+="ClinicNum = "+POut.Long(limitedBetaFeature.ClinicNum)+"";
			}
			if(limitedBetaFeature.IsSignedUp != oldLimitedBetaFeature.IsSignedUp) {
				if(command!="") { command+=",";}
				command+="IsSignedUp = "+POut.Bool(limitedBetaFeature.IsSignedUp)+"";
			}
			if(command=="") {
				return false;
			}
			command="UPDATE limitedbetafeature SET "+command
				+" WHERE LimitedBetaFeatureNum = "+POut.Long(limitedBetaFeature.LimitedBetaFeatureNum);
			Db.NonQ(command);
			return true;
		}

		///<summary>Returns true if Update(LimitedBetaFeature,LimitedBetaFeature) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(LimitedBetaFeature limitedBetaFeature,LimitedBetaFeature oldLimitedBetaFeature) {
			if(limitedBetaFeature.LimitedBetaFeatureTypeNum != oldLimitedBetaFeature.LimitedBetaFeatureTypeNum) {
				return true;
			}
			if(limitedBetaFeature.ClinicNum != oldLimitedBetaFeature.ClinicNum) {
				return true;
			}
			if(limitedBetaFeature.IsSignedUp != oldLimitedBetaFeature.IsSignedUp) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one LimitedBetaFeature from the database.</summary>
		public static void Delete(long limitedBetaFeatureNum) {
			string command="DELETE FROM limitedbetafeature "
				+"WHERE LimitedBetaFeatureNum = "+POut.Long(limitedBetaFeatureNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many LimitedBetaFeatures from the database.</summary>
		public static void DeleteMany(List<long> listLimitedBetaFeatureNums) {
			if(listLimitedBetaFeatureNums==null || listLimitedBetaFeatureNums.Count==0) {
				return;
			}
			string command="DELETE FROM limitedbetafeature "
				+"WHERE LimitedBetaFeatureNum IN("+string.Join(",",listLimitedBetaFeatureNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}