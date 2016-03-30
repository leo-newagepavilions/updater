using NLog;
using PatchClient.ClinentDbModel.DdModel;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace updaterData
{
    public class updaterClientData
    {        
        private static Container _container = new Container();
        private static ILogger _log;
        public updaterClientData()
        {
            _container.Register<ILogger>(() => LogManager.GetCurrentClassLogger(), Lifestyle.Singleton);
            _log = _container.GetInstance<ILogger>();
        }


        public IEnumerable<NAPClientPatch> GetAllClientPatchData()
        {
            try
            {
                using (var dbContext = new updaterDataContext())
                {
                    return dbContext.NAPClientPatches.Select(x => x).OrderByDescending(y => y.DownloadDate);
                }
            }
            catch(Exception ex)
            {
                _log.Error(ex, "Failed to get client patch data.");
            }
            return null;    
        }

        public NAPClientPatch GetClientPatchDataById(int PatchId)
        {
            try
            {
                using (var dbContext = new updaterDataContext())
                {
                    return dbContext.NAPClientPatches.FirstOrDefault(x => x.PatchScriptId == PatchId);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Failed to get client patch data by {0}.",PatchId);
            }
            return null;
        }

        public NAPClientPatch UpdateClientPatchDataByPatchId(int patchId)
        {

            return null;
        }
    }
}
