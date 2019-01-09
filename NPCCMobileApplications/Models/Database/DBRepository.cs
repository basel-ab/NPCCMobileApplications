﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;
using static NPCCMobileApplications.Library.npcc_types;

namespace NPCCMobileApplications.Library
{
    public class DBRepository
    {
        string dbPath;
        public DBRepository()
        {
            dbPath = Path.Combine(System.Environment.GetFolderPath
                                  (System.Environment.SpecialFolder.Personal), "ormdemo.db");
        }

        public bool CreateTable()
        {
            try
            {
                var cn = new SQLiteConnection(dbPath);
                cn.CreateTable<Spools>();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> RefreshSpoolAsync()
        {
            try
            {
                string url = "https://webapps.npcc.ae/ApplicationWebServices/api/paperless/pendinglist";
                List<Spools> lstObjs = await npcc_services.inf_CallWebServiceAsync<List<Spools>, string>(inf_method.Get, url);
                var cn = new SQLiteConnection(dbPath);
                cn.DeleteAll<Spools>();
                cn.InsertAll(lstObjs);
                return true;
            }
            catch (Exception ex)
            {
                npcc_services.inf_mobile_exception_managerAsync(ex.Message);
                return false;
            }
        }

        public List<Spools> GetSpools()
        {
            try
            {
                var cn = new SQLiteConnection(dbPath);
                var Spools = cn.Table<Spools>().ToList();
                return Spools;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Spools GetSpool(int id)
        {
            //var emp = cn.Get<Spools>(id);
            var cn = new SQLiteConnection(dbPath);
            var spl = cn.Table<Spools>().Where(x => x.Id == id).FirstOrDefault();
            return spl;
        }

        public bool UpdateSpool(int id, Spools nSpool)
        {
            try
            {
                var cn = new SQLiteConnection(dbPath);
                var spl = cn.Get<Spools>(id);
                spl.cSpoolNo = nSpool.cSpoolNo;
                spl.cStationName = nSpool.cStationName;
                spl.icon = nSpool.icon;
                cn.Update(spl);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool DeleteSpool(int id)
        {
            try
            {
                var cn = new SQLiteConnection(dbPath);
                //cn.Delete<Spools>(id);
                var spl = cn.Get<Spools>(id);
                cn.Delete(spl);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}