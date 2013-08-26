using System.ComponentModel.Composition.Hosting;
using System.Linq;
using NUnit.Framework;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Imports.Newtonsoft.Json;

namespace ClassLibrary1
{
    [TestFixture]
    public class EnumTests
    {
        [Test]
        public void search_by_enum()
        {
            var store = GetStore();
            using (var session = store.OpenSession())
            {
                var foo = new Foo();
                session.Store(foo);
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var foo = session.Query<Foo>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .SingleOrDefault(x => x.Status == Status.Current);
                Assert.NotNull(foo);
            }
        }

        [Test]
        public void search_by_name()
        {
            var store = GetStore();
            using (var session = store.OpenSession())
            {
                var foo = new Foo { Name = "qwe" };
                session.Store(foo);
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var foo = session.Query<Foo>()
                    .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                    .SingleOrDefault(x => x.Name == "qwe");
                Assert.NotNull(foo);
            }
        }

        public DocumentStore GetStore()
        {
            var store = new EmbeddableDocumentStore {RunInMemory = true};

            store.Configuration.Catalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));

            store.Conventions.CustomizeJsonSerializer = s =>
            {
                s.TypeNameHandling = TypeNameHandling.Objects;
                s.DefaultValueHandling = DefaultValueHandling.Ignore;
                s.PreserveReferencesHandling = PreserveReferencesHandling.All;
            };

            store.Initialize();
            return store;
        }
    }

    public class Foo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
    }

    public enum Status
    {
        Current = 0,
        Archived = 1,
        Deleted = 2
    }
}
