using Domain;
using IDataAccess;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class NamespaceDataAccess(SimulatorDbContext context) : INamespaceDataAccess
{
    private readonly SimulatorDbContext _context = context;

    public void CreateNamespace(SimNamespace simNamespace)
    {
        _context.SimNamespaces.Add(simNamespace);
        _context.SaveChanges();
    }

    public bool NamespaceExistsById(Guid? id)
    {
        return _context.SimNamespaces.Any(c => c.Id == id);
    }

    public SimNamespace GetNamespaceById(Guid? id)
    {
        return _context.SimNamespaces.FirstOrDefault(c => c.Id == id);
    }

    public List<SimNamespace> GetAllNamespaces()
    {
        var namespaces = _context.SimNamespaces
            .Include(n => n.BaseNamespace)

            .Include(n => n.Elements)
                .ThenInclude(c => c.BaseClass)

            .Include(n => n.Elements)
                .ThenInclude(c => c.Attributes)
                    .ThenInclude(a => a.Reference)
            .Include(n => n.Elements)
                .ThenInclude(c => c.Attributes)
                    .ThenInclude(a => a.Instance)

            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.Parameters)
                        .ThenInclude(p => p.Reference)

            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.LocalVariables)
                        .ThenInclude(v => v.Reference)
            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.LocalVariables)
                        .ThenInclude(v => v.Instance)

            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.Invocations)
                        .ThenInclude(i => i.Signature)
                            .ThenInclude(s => s.Parameters)
                                .ThenInclude(p => p.Reference)
            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.Invocations)
                        .ThenInclude(i => i.Signature)
                            .ThenInclude(s => s.Parameters)
                                .ThenInclude(p => p.Instance)
            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.Invocations)
                        .ThenInclude(i => i.Signature)
                            .ThenInclude(s => s.ReturnType)

            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.Invocations)
                        .ThenInclude(i => i.Reference)

            .Include(n => n.Elements)
                .ThenInclude(c => c.Methods)
                    .ThenInclude(m => m.ReturnType)

            .Include(n => n.Elements)
                .ThenInclude(c => c.Implements)
                    .ThenInclude(i => i.Methods)
                        .ThenInclude(m => m.Parameters)
                            .ThenInclude(p => p.Reference)

            .AsSplitQuery()
            .ToList();

        foreach(var simNamespace in namespaces)
        {
            foreach(var simClass in simNamespace.Elements)
            {
                foreach(var method in simClass.Methods)
                {
                    method.Invocations = method.Invocations
                        .OrderBy(i => i.Index)
                        .ToList();

                    method.Parameters = method.Parameters
                        .OrderBy(p => p.Index)
                        .ToList();

                    foreach(var inv in method.Invocations)
                    {
                        if(inv.Signature?.Parameters != null)
                        {
                            inv.Signature.Parameters = inv.Signature.Parameters
                                .OrderBy(p => p.Index)
                                .ToList();
                        }

                        switch(inv.Reference)
                        {
                            case ReferenceParameter rp:
                                _context.Entry(rp).Reference(r => r.Reference).Query().Include(p => p.Reference).Load();
                                break;
                            case ReferenceVariable rv:
                                _context.Entry(rv).Reference(r => r.Reference).Query().Include(v => v.Reference).Include(v => v.Instance).Load();
                                break;
                            case ReferenceAttribute ra:
                                _context.Entry(ra).Reference(r => r.Reference).Query().Include(a => a.Reference).Include(a => a.Instance).Load();
                                break;
                            case ReferenceBase rb:
                                _context.Entry(rb).Reference(r => r.Reference).Query().Include(c => c.BaseClass).Load();
                                break;
                            case ReferenceThis rt:
                                _context.Entry(rt).Reference(r => r.Reference).Load();
                                break;
                            case ReferenceStaticAttribute rsa:
                                _context.Entry(rsa).Reference(r => r.Reference).Query().Include(a => a.Reference).Include(a => a.Instance).Load();
                                break;
                            case ReferenceStatic rsv:
                                _context.Entry(rsv).Reference(r => r.Reference).Load();
                                break;
                        }
                    }
                }
            }
        }

        return namespaces;
    }

    public bool NamespaceExistsByName(string name)
    {
        return _context.SimNamespaces.Any(c => c.Name == name);
    }
}
