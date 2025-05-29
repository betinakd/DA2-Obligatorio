using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Context;

public class SimulatorDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<SimClass> SimClasses { get; set; }
    public DbSet<SimMethod> SimMethods { get; set; }
    public DbSet<SimAttribute> SimAttributes { get; set; }
    public DbSet<Parameter> Parameters { get; set; }
    public DbSet<LocalVariable> LocalVariables { get; set; }
    public DbSet<Invocation> Invocations { get; set; }
    public DbSet<Reference> References { get; set; }
    public DbSet<Signature> Signatures { get; set; }

    public DbSet<ParameterSignature> ParameterSignatures { get; set; }
    public DbSet<ExecutionLog> ExecutionLogs { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Configuration(modelBuilder);
        DataSeed(modelBuilder);
    }

    private void Configuration(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.Entity<SimClass>()
            .HasOne(s => s.BaseClass)
            .WithMany()
            .HasForeignKey(b => b.BaseClassId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimClass>()
            .HasMany(s => s.Methods)
            .WithOne(m => m.RelatedClass)
            .HasForeignKey(m => m.RelatedClassId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimClass>()
            .HasMany(s => s.Attributes)
            .WithOne(m => m.RelatedClass)
            .HasForeignKey(m => m.RelatedClassId);

        _ = modelBuilder.Entity<SimAttribute>()
                    .HasOne(a => a.RelatedClass)
                    .WithMany(c => c.Attributes)
                    .HasForeignKey(a => a.RelatedClassId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasOne(m => m.RelatedClass)
                    .WithMany(c => c.Methods)
                    .HasForeignKey(m => m.RelatedClassId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasMany(m => m.Parameters)
                    .WithOne(p => p.RelatedMethod)
                    .HasForeignKey(p => p.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasMany(m => m.LocalVariables)
                    .WithOne(v => v.RelatedMethod)
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<Parameter>()
                    .HasOne(p => p.RelatedMethod)
                    .WithMany(m => m.Parameters)
                    .HasForeignKey(p => p.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Parameter>()
                    .HasOne(p => p.Type)
                    .WithMany()
                    .HasForeignKey(p => p.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<LocalVariable>()
                    .HasOne(v => v.RelatedMethod)
                    .WithMany(m => m.LocalVariables)
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<LocalVariable>()
                    .HasOne(v => v.Reference)
                    .WithMany()
                    .HasForeignKey(v => v.ReferenceId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasMany(i => i.Invocations)
                    .WithOne()
                    .HasForeignKey(v => v.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder.Entity<SimMethod>()
                    .HasOne(m => m.ReturnType)
                    .WithMany()
                    .HasForeignKey(m => m.ReturnTypeId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<SimAttribute>()
                    .HasOne(a => a.Reference)
                    .WithMany()
                    .HasForeignKey(b => b.ReferenceId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
                    .HasOne(i => i.RelatedMethod)
                    .WithMany(m => m.Invocations)
                    .HasForeignKey(i => i.RelatedMethodId)
                    .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
            .HasOne(i => i.Reference)
            .WithOne(r => r.RelatedInvocation)
            .HasForeignKey<Invocation>(i => i.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
            .HasOne(i => i.Signature)
            .WithOne(s => s.RelatedInvocation)
            .HasForeignKey<Invocation>(i => i.SignatureId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
            .HasOne(m => m.Signature)
            .WithOne(r => r.RelatedInvocation)
            .HasForeignKey<Signature>(r => r.RelatedInvocationId);

        _ = modelBuilder.Entity<Reference>()
                    .ToTable("References")
                    .HasDiscriminator<string>("ReferenceType")
                    .HasValue<ReferenceThis>("This")
                    .HasValue<ReferenceBase>("Base")
                    .HasValue<ReferenceAttribute>("Attribute")
                    .HasValue<ReferenceParameter>("Parameter")
                    .HasValue<ReferenceVariable>("Variable")
                    .HasValue<ReferenceStaticAttribute>("StaticAttribute")
                    .HasValue<ReferenceStatic>("Static");

        _ = modelBuilder.Entity<Reference>()
            .HasKey(r => r.Id);

        _ = modelBuilder.Entity<ReferenceThis>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceAttribute>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceParameter>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceVariable>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ReferenceBase>()
            .HasOne(r => r.Reference)
            .WithMany()
            .HasForeignKey(r => r.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Signature>()
            .HasMany(s => s.Parameters)
            .WithOne(ps => ps.Signature)
            .HasForeignKey(ps => ps.SignatureId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<ParameterSignature>()
            .HasOne(ps => ps.Reference)
            .WithMany()
            .HasForeignKey(ps => ps.ReferenceId)
            .OnDelete(DeleteBehavior.Restrict);

        _ = modelBuilder.Entity<Invocation>()
            .Property(i => i.Index)
            .HasDefaultValue(0);

        _ = modelBuilder.Entity<Parameter>()
            .Property(p => p.Index)
            .HasDefaultValue(0);

        _ = modelBuilder.Entity<ParameterSignature>()
            .Property(p => p.Index)
            .HasDefaultValue(0);

        _ = modelBuilder.Entity<SimClass>()
            .HasMany(c => c.Implements)
            .WithMany()
            .UsingEntity(j => j.ToTable("SimClassImplements"));

        _ = modelBuilder.Entity<ApiKey>(entity =>
            {
                entity.HasKey(e => e.KeyValue);
                entity.Property(e => e.Name).IsRequired();
            });
    }

    private void DataSeed(ModelBuilder modelBuilder)
    {
        var objectClassId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var voidTypeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var boolTypeId = Guid.Parse("22223222-2222-2222-2222-222222222222");
        var stringTypeId = Guid.Parse("44444444-1111-1111-1111-111111111111");

        var byteTypeId = Guid.Parse("33333333-1111-1111-1111-111111111111");
        var sbyteTypeId = Guid.Parse("33333333-2222-1111-1111-111111111111");
        var charTypeId = Guid.Parse("33333333-3333-1111-1111-111111111111");
        var decimalTypeId = Guid.Parse("33333333-4444-1111-1111-111111111111");
        var doubleTypeId = Guid.Parse("33333333-5555-1111-1111-111111111111");
        var floatTypeId = Guid.Parse("33333333-6666-1111-1111-111111111111");
        var intTypeId = Guid.Parse("33333333-7777-1111-1111-111111111111");
        var uintTypeId = Guid.Parse("33333333-8888-1111-1111-111111111111");
        var nintTypeId = Guid.Parse("33333333-9999-1111-1111-111111111111");
        var nuintTypeId = Guid.Parse("33333333-AAAA-1111-1111-111111111111");
        var longTypeId = Guid.Parse("33333333-BBBB-1111-1111-111111111111");
        var ulongTypeId = Guid.Parse("33333333-CCCC-1111-1111-111111111111");
        var shortTypeId = Guid.Parse("33333333-DDDD-1111-1111-111111111111");
        var ushortTypeId = Guid.Parse("33333333-EEEE-1111-1111-111111111111");
        var delegateTypeId = Guid.Parse("44444444-2222-1111-1111-111111111111");
        var dynamicTypeId = Guid.Parse("44444444-3333-1111-1111-111111111111");

        _ = modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = objectClassId,
                Name = "Object",
                State = SimAccesibility.Normal,
                BaseClassId = null
            },
            new SimClass
            {
                Id = voidTypeId,
                Name = "void",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            });

        _ = modelBuilder.Entity<SimClass>().HasData(
            new SimClass
            {
                Id = boolTypeId,
                Name = "bool",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = byteTypeId,
                Name = "byte",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = sbyteTypeId,
                Name = "sbyte",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = charTypeId,
                Name = "char",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = decimalTypeId,
                Name = "decimal",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = doubleTypeId,
                Name = "double",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = floatTypeId,
                Name = "float",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = intTypeId,
                Name = "int",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = uintTypeId,
                Name = "uint",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = nintTypeId,
                Name = "nint",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = nuintTypeId,
                Name = "nuint",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = longTypeId,
                Name = "long",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = ulongTypeId,
                Name = "ulong",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = shortTypeId,
                Name = "short",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = ushortTypeId,
                Name = "ushort",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = stringTypeId,
                Name = "string",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = delegateTypeId,
                Name = "delegate",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            },
            new SimClass
            {
                Id = dynamicTypeId,
                Name = "dynamic",
                BaseClassId = objectClassId,
                State = SimAccesibility.Normal,
            });

        var equalsMethodId = Guid.Parse("55555555-1111-1111-1111-111111111111");
        var equalsStaticMethodId = Guid.Parse("55555555-2222-1111-1111-111111111111");
        var finalizeMethodId = Guid.Parse("55555555-3333-1111-1111-111111111111");
        var getHashCodeMethodId = Guid.Parse("55555555-4444-1111-1111-111111111111");
        var getTypeMethodId = Guid.Parse("55555555-5555-1111-1111-111111111111");
        var memberWiseCloneMethodId = Guid.Parse("55555555-6666-1111-1111-111111111111");
        var referenceEqualsMethodId = Guid.Parse("55555555-7777-1111-1111-111111111111");
        var toStringMethodId = Guid.Parse("55555555-8888-1111-1111-111111111111");

        var equalsObjParamId = Guid.Parse("66666666-1111-1111-1111-111111111111");
        var equalsStaticObj1ParamId = Guid.Parse("66666666-2222-1111-1111-111111111111");
        var equalsStaticObj2ParamId = Guid.Parse("66666666-3333-1111-1111-111111111111");
        var referenceEqualsObj1ParamId = Guid.Parse("66666666-4444-1111-1111-111111111111");
        var referenceEqualsObj2ParamId = Guid.Parse("66666666-5555-1111-1111-111111111111");

        _ = modelBuilder.Entity<SimMethod>().HasData(
            new SimMethod
            {
                Id = equalsMethodId,
                Name = "Equals",
                ReturnTypeId = boolTypeId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            },
            new SimMethod
            {
                Id = equalsStaticMethodId,
                Name = "Equals",
                ReturnTypeId = boolTypeId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            },
            new SimMethod
            {
                Id = finalizeMethodId,
                Name = "Finalize",
                ReturnTypeId = voidTypeId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Protected,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            },
            new SimMethod
            {
                Id = getHashCodeMethodId,
                Name = "GetHashCode",
                ReturnTypeId = intTypeId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            },
            new SimMethod
            {
                Id = getTypeMethodId,
                Name = "GetType",
                ReturnTypeId = objectClassId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            },
            new SimMethod
            {
                Id = memberWiseCloneMethodId,
                Name = "MemberwiseClone",
                ReturnTypeId = objectClassId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Protected,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            },
            new SimMethod
            {
                Id = referenceEqualsMethodId,
                Name = "ReferenceEquals",
                ReturnTypeId = boolTypeId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            },
            new SimMethod
            {
                Id = toStringMethodId,
                Name = "ToString",
                ReturnTypeId = stringTypeId,
                RelatedClassId = objectClassId,
                Privacity = SimPrivacity.Public,
                Accesibility = SimAccesibility.Normal,
                IsStatic = false,
                IsVirtual = true,
                IsOverride = true
            });

        _ = modelBuilder.Entity<Parameter>().HasData(
        new Parameter
        {
            Id = equalsObjParamId,
            Name = "obj",
            TypeId = objectClassId,
            RelatedMethodId = equalsMethodId,
            Index = 0
        },
        new Parameter
        {
            Id = equalsStaticObj1ParamId,
            Name = "objA",
            TypeId = objectClassId,
            RelatedMethodId = equalsStaticMethodId,
            Index = 0
        },
        new Parameter
        {
            Id = equalsStaticObj2ParamId,
            Name = "objB",
            TypeId = objectClassId,
            RelatedMethodId = equalsStaticMethodId,
            Index = 1
        },
        new Parameter
        {
            Id = referenceEqualsObj1ParamId,
            Name = "objA",
            TypeId = objectClassId,
            RelatedMethodId = referenceEqualsMethodId,
            Index = 0
        },
        new Parameter
        {
            Id = referenceEqualsObj2ParamId,
            Name = "objB",
            TypeId = objectClassId,
            RelatedMethodId = referenceEqualsMethodId,
            Index = 1
        });

        _ = modelBuilder.Entity<ApiKey>().HasData(
            new ApiKey
            {
                KeyValue = Guid.Parse("77777777-aaaa-1111-1111-111111111111"),
                Name = "validKey_1"
            },
            new ApiKey
            {
                KeyValue = Guid.Parse("77777777-bbbb-1111-1111-111111111111"),
                Name = "validKey_2"
            });
    }
}
