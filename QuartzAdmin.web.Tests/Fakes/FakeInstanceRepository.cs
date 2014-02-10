using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuartzAdmin.web.Models;

namespace QuartzAdmin.web.Tests.Fakes
{
    public class FakeInstanceRepository : IInstanceRepository
    {
        #region IInstanceRepository Members

        List<InstanceModel> _instances = new List<InstanceModel>();

        public void Delete(InstanceModel instance)
        {
            _instances.Remove(instance);
        }

        public void Save(InstanceModel instance)
        {
            //throw new NotImplementedException();
            _instances.Add(instance);
            InstanceModel found = _instances.Find(x => x.InstanceName == instance.InstanceName);
            if (found != null)
            {
                _instances.Remove(found);
            }
            _instances.Add(instance);
        }

        public InstanceModel GetByName(string name)
        {
            //throw new NotImplementedException();

            return _instances.Where(x => x.InstanceName == name).FirstOrDefault();
        }


        public List<InstanceModel> GetAll()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
