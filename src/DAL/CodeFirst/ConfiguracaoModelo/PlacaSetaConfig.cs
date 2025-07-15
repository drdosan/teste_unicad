
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using System.ComponentModel.DataAnnotations.Schema;

using Raizen.UniCad.Model;

namespace Raizen.UniCad.DAL.CodeFirst
{
    public class PlacaSetaConfig : EntityTypeConfiguration<PlacaSeta>
    {
        public PlacaSetaConfig()
        {
            this.ToTable("PlacaSeta");
            this.HasKey(chave => chave.ID);
            this.Property(t => t.ID).HasColumnName("ID").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.IDPlaca).HasColumnName("IDPlaca");
            this.Property(t => t.Volume).HasColumnName("Volume");
            this.Property(t => t.VolumeCompartimento1).HasColumnName("VolumeCompartimento1");
            this.Property(t => t.VolumeCompartimento2).HasColumnName("VolumeCompartimento2");
            this.Property(t => t.VolumeCompartimento3).HasColumnName("VolumeCompartimento3");
            this.Property(t => t.VolumeCompartimento4).HasColumnName("VolumeCompartimento4");
            this.Property(t => t.VolumeCompartimento5).HasColumnName("VolumeCompartimento5");
            this.Property(t => t.VolumeCompartimento6).HasColumnName("VolumeCompartimento6");
            this.Property(t => t.VolumeCompartimento7).HasColumnName("VolumeCompartimento7");
            this.Property(t => t.VolumeCompartimento8).HasColumnName("VolumeCompartimento8");
            this.Property(t => t.VolumeCompartimento9).HasColumnName("VolumeCompartimento9");
            this.Property(t => t.VolumeCompartimento10).HasColumnName("VolumeCompartimento10");
            this.Property(t => t.LacreCompartimento1).HasColumnName("LacreCompartimento1");
            this.Property(t => t.LacreCompartimento2).HasColumnName("LacreCompartimento2");
            this.Property(t => t.LacreCompartimento3).HasColumnName("LacreCompartimento3");
            this.Property(t => t.LacreCompartimento4).HasColumnName("LacreCompartimento4");
            this.Property(t => t.LacreCompartimento5).HasColumnName("LacreCompartimento5");
            this.Property(t => t.LacreCompartimento6).HasColumnName("LacreCompartimento6");
            this.Property(t => t.LacreCompartimento7).HasColumnName("LacreCompartimento7");
            this.Property(t => t.LacreCompartimento8).HasColumnName("LacreCompartimento8");
            this.Property(t => t.LacreCompartimento9).HasColumnName("LacreCompartimento9");
            this.Property(t => t.LacreCompartimento10).HasColumnName("LacreCompartimento10");
            this.Property(t => t.Compartimento1IsInativo).HasColumnName("Compartimento1IsInativo");
            this.Property(t => t.Compartimento2IsInativo).HasColumnName("Compartimento2IsInativo");
            this.Property(t => t.Compartimento3IsInativo).HasColumnName("Compartimento3IsInativo");
            this.Property(t => t.Compartimento4IsInativo).HasColumnName("Compartimento4IsInativo");
            this.Property(t => t.Compartimento5IsInativo).HasColumnName("Compartimento5IsInativo");
            this.Property(t => t.Compartimento6IsInativo).HasColumnName("Compartimento6IsInativo");
            this.Property(t => t.Compartimento7IsInativo).HasColumnName("Compartimento7IsInativo");
            this.Property(t => t.Compartimento8IsInativo).HasColumnName("Compartimento8IsInativo");
            this.Property(t => t.Compartimento9IsInativo).HasColumnName("Compartimento9IsInativo");
            this.Property(t => t.Compartimento10IsInativo).HasColumnName("Compartimento10IsInativo");
        }
    }
}

