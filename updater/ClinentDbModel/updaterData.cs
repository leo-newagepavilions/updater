using NLog;
using PatchClient.ClinentDbModel.DataModel;
using PatchClient.MCPSvc;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace updaterData
{
    public enum eLocalState
    {
        Init = 0,
        Extracted = Init << 1,
        Applied = Init << 2
    }


    public class updaterClientData
    {        
        private static Container _container = new Container();
        private static ILogger _log;
        private NAPClientDbContext dbContext = new NAPClientDbContext();       
        public updaterClientData()
        {
            //_container.Register<ILogger>(() => LogManager.GetCurrentClassLogger(), Lifestyle.Singleton);
            //_log = _container.GetInstance<ILogger>();
        }


        public IEnumerable<PatchClient.ClinentDbModel.DataModel.NAPClientPatch> GetAllClientPatchData()
        {
            try
            {
                using (var dbContext = new NAPClientDbContext())
                {
                    return dbContext.NAPClientPatches.Select(x => x).OrderByDescending(y => y.DownloadDate);
                }
            }
            catch(Exception ex)
            {
                _log.Error(ex, "Failed to get client patch data.");
                throw;
            }               
        }

        public NAPClientPatch GetClientPatchDataById(int PatchId)
        {
            try
            {
                using (var dbContext = new NAPClientDbContext())
                {
                    return dbContext.NAPClientPatches.FirstOrDefault(x => x.PatchScriptId == PatchId);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to get client patch data by {0}.",PatchId);
                throw;
            }
        }

        public NAPClientPatch AddClientPatchData(NAPClientPatch patchatClient)
        {
            try
            {
                using (var dbContext = new NAPClientDbContext())
                {
                    dbContext.Configuration.AutoDetectChangesEnabled = true;
                    dbContext.Configuration.LazyLoadingEnabled = false;
                   var result = dbContext.NAPClientPatches.Add(patchatClient);
                    dbContext.SaveChanges();
                    return result;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to get client patch data by{0}", patchatClient);
                throw;
            }
        }

        public bool UpdateClientPatchData(NAPClientPatch patchatClient)
        {
            try
            {
                using (var dbContext = new NAPClientDbContext())
                {
                    //dbContext.Configuration.AutoDetectChangesEnabled = true;
                    //dbContext.Configuration.LazyLoadingEnabled = false;
                    var find = dbContext.NAPClientPatches.FirstOrDefault(x => x.PatchScriptId == patchatClient.PatchScriptId);                    
                    if (find == null) return false;                    
                    dbContext.Set<NAPClientPatch>().AddOrUpdate(patchatClient);
                    dbContext.SaveChanges();
                    return true;                 
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to get client patch data by{0}", patchatClient);
                throw;
            }
        }

        public IEnumerable<NAPClientPatch> GetAllNewClientPatchData()
        {
            try
            {
                //return dbContext.NAPClientPatches.Select(x => x);
                return dbContext.NAPClientPatches.Where(x => x.PatchState == "Downloaded");
                
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to get client patch data by{0}");
                throw;
            }
        }
        
    }
}
