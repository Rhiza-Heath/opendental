//This file is automatically generated.
//Do not attempt to make changes to this file because the changes will be erased and overwritten.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace OpenDentBusiness.Crud{
	public class MountCrud {
		///<summary>Gets one Mount object from the database using the primary key.  Returns null if not found.</summary>
		public static Mount SelectOne(long mountNum) {
			string command="SELECT * FROM mount "
				+"WHERE MountNum = "+POut.Long(mountNum);
			List<Mount> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets one Mount object from the database using a query.</summary>
		public static Mount SelectOne(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Mount> list=TableToList(Db.GetTable(command));
			if(list.Count==0) {
				return null;
			}
			return list[0];
		}

		///<summary>Gets a list of Mount objects from the database using a query.</summary>
		public static List<Mount> SelectMany(string command) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				throw new ApplicationException("Not allowed to send sql directly.  Rewrite the calling class to not use this query:\r\n"+command);
			}
			List<Mount> list=TableToList(Db.GetTable(command));
			return list;
		}

		///<summary>Converts a DataTable to a list of objects.</summary>
		public static List<Mount> TableToList(DataTable table) {
			List<Mount> retVal=new List<Mount>();
			Mount mount;
			foreach(DataRow row in table.Rows) {
				mount=new Mount();
				mount.MountNum          = PIn.Long  (row["MountNum"].ToString());
				mount.PatNum            = PIn.Long  (row["PatNum"].ToString());
				mount.DocCategory       = PIn.Long  (row["DocCategory"].ToString());
				mount.DateCreated       = PIn.DateT (row["DateCreated"].ToString());
				mount.Description       = PIn.String(row["Description"].ToString());
				mount.Note              = PIn.String(row["Note"].ToString());
				mount.Width             = PIn.Int   (row["Width"].ToString());
				mount.Height            = PIn.Int   (row["Height"].ToString());
				mount.ColorBack         = Color.FromArgb(PIn.Int(row["ColorBack"].ToString()));
				mount.ProvNum           = PIn.Long  (row["ProvNum"].ToString());
				mount.ColorFore         = Color.FromArgb(PIn.Int(row["ColorFore"].ToString()));
				mount.ColorTextBack     = Color.FromArgb(PIn.Int(row["ColorTextBack"].ToString()));
				mount.FlipOnAcquire     = PIn.Bool  (row["FlipOnAcquire"].ToString());
				mount.AdjModeAfterSeries= PIn.Bool  (row["AdjModeAfterSeries"].ToString());
				retVal.Add(mount);
			}
			return retVal;
		}

		///<summary>Converts a list of Mount into a DataTable.</summary>
		public static DataTable ListToTable(List<Mount> listMounts,string tableName="") {
			if(string.IsNullOrEmpty(tableName)) {
				tableName="Mount";
			}
			DataTable table=new DataTable(tableName);
			table.Columns.Add("MountNum");
			table.Columns.Add("PatNum");
			table.Columns.Add("DocCategory");
			table.Columns.Add("DateCreated");
			table.Columns.Add("Description");
			table.Columns.Add("Note");
			table.Columns.Add("Width");
			table.Columns.Add("Height");
			table.Columns.Add("ColorBack");
			table.Columns.Add("ProvNum");
			table.Columns.Add("ColorFore");
			table.Columns.Add("ColorTextBack");
			table.Columns.Add("FlipOnAcquire");
			table.Columns.Add("AdjModeAfterSeries");
			foreach(Mount mount in listMounts) {
				table.Rows.Add(new object[] {
					POut.Long  (mount.MountNum),
					POut.Long  (mount.PatNum),
					POut.Long  (mount.DocCategory),
					POut.DateT (mount.DateCreated,false),
					            mount.Description,
					            mount.Note,
					POut.Int   (mount.Width),
					POut.Int   (mount.Height),
					POut.Int   (mount.ColorBack.ToArgb()),
					POut.Long  (mount.ProvNum),
					POut.Int   (mount.ColorFore.ToArgb()),
					POut.Int   (mount.ColorTextBack.ToArgb()),
					POut.Bool  (mount.FlipOnAcquire),
					POut.Bool  (mount.AdjModeAfterSeries),
				});
			}
			return table;
		}

		///<summary>Inserts one Mount into the database.  Returns the new priKey.</summary>
		public static long Insert(Mount mount) {
			return Insert(mount,false);
		}

		///<summary>Inserts one Mount into the database.  Provides option to use the existing priKey.</summary>
		public static long Insert(Mount mount,bool useExistingPK) {
			if(!useExistingPK && PrefC.RandomKeys) {
				mount.MountNum=ReplicationServers.GetKey("mount","MountNum");
			}
			string command="INSERT INTO mount (";
			if(useExistingPK || PrefC.RandomKeys) {
				command+="MountNum,";
			}
			command+="PatNum,DocCategory,DateCreated,Description,Note,Width,Height,ColorBack,ProvNum,ColorFore,ColorTextBack,FlipOnAcquire,AdjModeAfterSeries) VALUES(";
			if(useExistingPK || PrefC.RandomKeys) {
				command+=POut.Long(mount.MountNum)+",";
			}
			command+=
				     POut.Long  (mount.PatNum)+","
				+    POut.Long  (mount.DocCategory)+","
				+    POut.DateT (mount.DateCreated)+","
				+"'"+POut.String(mount.Description)+"',"
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.Int   (mount.Width)+","
				+    POut.Int   (mount.Height)+","
				+    POut.Int   (mount.ColorBack.ToArgb())+","
				+    POut.Long  (mount.ProvNum)+","
				+    POut.Int   (mount.ColorFore.ToArgb())+","
				+    POut.Int   (mount.ColorTextBack.ToArgb())+","
				+    POut.Bool  (mount.FlipOnAcquire)+","
				+    POut.Bool  (mount.AdjModeAfterSeries)+")";
			if(mount.Note==null) {
				mount.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(mount.Note));
			if(useExistingPK || PrefC.RandomKeys) {
				Db.NonQ(command,paramNote);
			}
			else {
				mount.MountNum=Db.NonQ(command,true,"MountNum","mount",paramNote);
			}
			return mount.MountNum;
		}

		///<summary>Inserts one Mount into the database.  Returns the new priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Mount mount) {
			return InsertNoCache(mount,false);
		}

		///<summary>Inserts one Mount into the database.  Provides option to use the existing priKey.  Doesn't use the cache.</summary>
		public static long InsertNoCache(Mount mount,bool useExistingPK) {
			bool isRandomKeys=Prefs.GetBoolNoCache(PrefName.RandomPrimaryKeys);
			string command="INSERT INTO mount (";
			if(!useExistingPK && isRandomKeys) {
				mount.MountNum=ReplicationServers.GetKeyNoCache("mount","MountNum");
			}
			if(isRandomKeys || useExistingPK) {
				command+="MountNum,";
			}
			command+="PatNum,DocCategory,DateCreated,Description,Note,Width,Height,ColorBack,ProvNum,ColorFore,ColorTextBack,FlipOnAcquire,AdjModeAfterSeries) VALUES(";
			if(isRandomKeys || useExistingPK) {
				command+=POut.Long(mount.MountNum)+",";
			}
			command+=
				     POut.Long  (mount.PatNum)+","
				+    POut.Long  (mount.DocCategory)+","
				+    POut.DateT (mount.DateCreated)+","
				+"'"+POut.String(mount.Description)+"',"
				+    DbHelper.ParamChar+"paramNote,"
				+    POut.Int   (mount.Width)+","
				+    POut.Int   (mount.Height)+","
				+    POut.Int   (mount.ColorBack.ToArgb())+","
				+    POut.Long  (mount.ProvNum)+","
				+    POut.Int   (mount.ColorFore.ToArgb())+","
				+    POut.Int   (mount.ColorTextBack.ToArgb())+","
				+    POut.Bool  (mount.FlipOnAcquire)+","
				+    POut.Bool  (mount.AdjModeAfterSeries)+")";
			if(mount.Note==null) {
				mount.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(mount.Note));
			if(useExistingPK || isRandomKeys) {
				Db.NonQ(command,paramNote);
			}
			else {
				mount.MountNum=Db.NonQ(command,true,"MountNum","mount",paramNote);
			}
			return mount.MountNum;
		}

		///<summary>Updates one Mount in the database.</summary>
		public static void Update(Mount mount) {
			string command="UPDATE mount SET "
				+"PatNum            =  "+POut.Long  (mount.PatNum)+", "
				+"DocCategory       =  "+POut.Long  (mount.DocCategory)+", "
				+"DateCreated       =  "+POut.DateT (mount.DateCreated)+", "
				+"Description       = '"+POut.String(mount.Description)+"', "
				+"Note              =  "+DbHelper.ParamChar+"paramNote, "
				+"Width             =  "+POut.Int   (mount.Width)+", "
				+"Height            =  "+POut.Int   (mount.Height)+", "
				+"ColorBack         =  "+POut.Int   (mount.ColorBack.ToArgb())+", "
				+"ProvNum           =  "+POut.Long  (mount.ProvNum)+", "
				+"ColorFore         =  "+POut.Int   (mount.ColorFore.ToArgb())+", "
				+"ColorTextBack     =  "+POut.Int   (mount.ColorTextBack.ToArgb())+", "
				+"FlipOnAcquire     =  "+POut.Bool  (mount.FlipOnAcquire)+", "
				+"AdjModeAfterSeries=  "+POut.Bool  (mount.AdjModeAfterSeries)+" "
				+"WHERE MountNum = "+POut.Long(mount.MountNum);
			if(mount.Note==null) {
				mount.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(mount.Note));
			Db.NonQ(command,paramNote);
		}

		///<summary>Updates one Mount in the database.  Uses an old object to compare to, and only alters changed fields.  This prevents collisions and concurrency problems in heavily used tables.  Returns true if an update occurred.</summary>
		public static bool Update(Mount mount,Mount oldMount) {
			string command="";
			if(mount.PatNum != oldMount.PatNum) {
				if(command!="") { command+=",";}
				command+="PatNum = "+POut.Long(mount.PatNum)+"";
			}
			if(mount.DocCategory != oldMount.DocCategory) {
				if(command!="") { command+=",";}
				command+="DocCategory = "+POut.Long(mount.DocCategory)+"";
			}
			if(mount.DateCreated != oldMount.DateCreated) {
				if(command!="") { command+=",";}
				command+="DateCreated = "+POut.DateT(mount.DateCreated)+"";
			}
			if(mount.Description != oldMount.Description) {
				if(command!="") { command+=",";}
				command+="Description = '"+POut.String(mount.Description)+"'";
			}
			if(mount.Note != oldMount.Note) {
				if(command!="") { command+=",";}
				command+="Note = "+DbHelper.ParamChar+"paramNote";
			}
			if(mount.Width != oldMount.Width) {
				if(command!="") { command+=",";}
				command+="Width = "+POut.Int(mount.Width)+"";
			}
			if(mount.Height != oldMount.Height) {
				if(command!="") { command+=",";}
				command+="Height = "+POut.Int(mount.Height)+"";
			}
			if(mount.ColorBack != oldMount.ColorBack) {
				if(command!="") { command+=",";}
				command+="ColorBack = "+POut.Int(mount.ColorBack.ToArgb())+"";
			}
			if(mount.ProvNum != oldMount.ProvNum) {
				if(command!="") { command+=",";}
				command+="ProvNum = "+POut.Long(mount.ProvNum)+"";
			}
			if(mount.ColorFore != oldMount.ColorFore) {
				if(command!="") { command+=",";}
				command+="ColorFore = "+POut.Int(mount.ColorFore.ToArgb())+"";
			}
			if(mount.ColorTextBack != oldMount.ColorTextBack) {
				if(command!="") { command+=",";}
				command+="ColorTextBack = "+POut.Int(mount.ColorTextBack.ToArgb())+"";
			}
			if(mount.FlipOnAcquire != oldMount.FlipOnAcquire) {
				if(command!="") { command+=",";}
				command+="FlipOnAcquire = "+POut.Bool(mount.FlipOnAcquire)+"";
			}
			if(mount.AdjModeAfterSeries != oldMount.AdjModeAfterSeries) {
				if(command!="") { command+=",";}
				command+="AdjModeAfterSeries = "+POut.Bool(mount.AdjModeAfterSeries)+"";
			}
			if(command=="") {
				return false;
			}
			if(mount.Note==null) {
				mount.Note="";
			}
			OdSqlParameter paramNote=new OdSqlParameter("paramNote",OdDbType.Text,POut.StringParam(mount.Note));
			command="UPDATE mount SET "+command
				+" WHERE MountNum = "+POut.Long(mount.MountNum);
			Db.NonQ(command,paramNote);
			return true;
		}

		///<summary>Returns true if Update(Mount,Mount) would make changes to the database.
		///Does not make any changes to the database and can be called before remoting role is checked.</summary>
		public static bool UpdateComparison(Mount mount,Mount oldMount) {
			if(mount.PatNum != oldMount.PatNum) {
				return true;
			}
			if(mount.DocCategory != oldMount.DocCategory) {
				return true;
			}
			if(mount.DateCreated != oldMount.DateCreated) {
				return true;
			}
			if(mount.Description != oldMount.Description) {
				return true;
			}
			if(mount.Note != oldMount.Note) {
				return true;
			}
			if(mount.Width != oldMount.Width) {
				return true;
			}
			if(mount.Height != oldMount.Height) {
				return true;
			}
			if(mount.ColorBack != oldMount.ColorBack) {
				return true;
			}
			if(mount.ProvNum != oldMount.ProvNum) {
				return true;
			}
			if(mount.ColorFore != oldMount.ColorFore) {
				return true;
			}
			if(mount.ColorTextBack != oldMount.ColorTextBack) {
				return true;
			}
			if(mount.FlipOnAcquire != oldMount.FlipOnAcquire) {
				return true;
			}
			if(mount.AdjModeAfterSeries != oldMount.AdjModeAfterSeries) {
				return true;
			}
			return false;
		}

		///<summary>Deletes one Mount from the database.</summary>
		public static void Delete(long mountNum) {
			string command="DELETE FROM mount "
				+"WHERE MountNum = "+POut.Long(mountNum);
			Db.NonQ(command);
		}

		///<summary>Deletes many Mounts from the database.</summary>
		public static void DeleteMany(List<long> listMountNums) {
			if(listMountNums==null || listMountNums.Count==0) {
				return;
			}
			string command="DELETE FROM mount "
				+"WHERE MountNum IN("+string.Join(",",listMountNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

	}
}