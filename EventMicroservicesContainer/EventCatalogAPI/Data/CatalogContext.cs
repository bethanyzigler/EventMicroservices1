using EventCatalogAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//This is the database context - definition of how 
//the database is defined

namespace EventCatalogAPI.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions options) : base(options)
        {
            //empty - upon entity call, calls base class
        }

        //three tables to be constructed:
        public DbSet<CatalogType> CatalogTypes { get; set; }
        public DbSet<CatalogCompany> CatalogCompanies { get; set; }
        public DbSet<CatalogEvent> CatalogEvents { get; set; }

        //over ride entity framework, when creating models, use these methods

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //upon model creation, use configure method (already written) to build
            modelBuilder.Entity<CatalogCompany>(ConfigureCatalogCompany);
            modelBuilder.Entity<CatalogType>(ConfigureCatalogType);
            modelBuilder.Entity<CatalogEvent>(ConfigureCatalogEvent);
        }

        //table=entity, builder- indicates primary key and converts to table
        private void ConfigureCatalogType(EntityTypeBuilder<CatalogType> builder)

        {
            builder.ToTable("CatalogType");
            builder.Property(c => c.Id)
                .IsRequired() //Id=required column
                .ForSqlServerUseSequenceHiLo("catalog_type_hilo"); //Id is auto generated
            builder.Property(c => c.Type)
                .IsRequired() //Type=required column
                .HasMaxLength(100);
        }

        private void ConfigureCatalogCompany(EntityTypeBuilder<CatalogCompany> builder)

        {
            builder.ToTable("CatalogCompany");
            builder.Property(c => c.Id)
                .IsRequired() //Id=required column
                .ForSqlServerUseSequenceHiLo("catalog_company_hilo"); //Id is auto generated
            builder.Property(c => c.Company)
                .IsRequired() //Company=required column
                .HasMaxLength(100);
        }

        private void ConfigureCatalogEvent(EntityTypeBuilder<CatalogEvent> builder)

        {
            builder.ToTable("Catalog");
            builder.Property(c => c.Id)
                .IsRequired() //Id=required column
                .ForSqlServerUseSequenceHiLo("catalog_hilo"); //Id is auto generated
            builder.Property(c => c.Name)
                .IsRequired() //Name=required column
                .HasMaxLength(50);
            builder.Property(c => c.Price)
                .IsRequired(); //Price=required column

            builder.HasOne(c => c.CatalogCompany) //catalog event builder has one company assigned to it
                .WithMany() //defines one to many relationship
                .HasForeignKey(c => c.CatalogCompanyId); //company Id = foreign key

            builder.HasOne(c => c.CatalogType) //catalog event builder has one type assigned to it
                .WithMany() //defined one to many relationship
                .HasForeignKey(c => c.CatalogTypeId); //type = foreign key






            ///Add in Description, PictureUrl from CatalogEvent Class?
        }
    }
}
