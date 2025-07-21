using System;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Contexts;

public class FeatureContext(DbContextOptions<FeatureContext> options) : DbContext(options)
{
    public DbSet<Feature> Features { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("features_pk");

            entity.ToTable("features");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id")
                .HasDefaultValueSql("nextval('locations_id_seq'::regclass)");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Wkt)
                .HasColumnType("character varying")
                .HasColumnName("wkt");
        });

        modelBuilder.HasSequence("locations_id_seq", "id");
    }
}