using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using ThesisTestAPI.Entities;

namespace ThesisTestAPI

{
    public class DatabaseXmlRepository : IXmlRepository
    {
        private readonly ThesisDbContext _db;
        public DatabaseXmlRepository(ThesisDbContext db)
        {
            _db = db;
        }
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return _db.DataProtectionKeys
            .AsNoTracking()
            .Select(k => XElement.Parse(k.XmlData))
            .ToList()
            .AsReadOnly();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            var xmlString = element.ToString();
            _db.DataProtectionKeys.Add(new DataProtectionKey { XmlData = xmlString });
            _db.SaveChanges();
        }
    }
}
