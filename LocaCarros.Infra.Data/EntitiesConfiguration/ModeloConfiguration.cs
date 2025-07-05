using LocaCarros.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocaCarros.Infra.Data.EntitiesConfiguration
{
    public class ModeloConfiguration: IEntityTypeConfiguration<Modelo>
    {
        public void Configure(EntityTypeBuilder<Modelo> builder)
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Nome).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Versao).IsRequired().HasMaxLength(50);
            builder.Property(m => m.Motorizacao).IsRequired().HasColumnType("decimal(5,2)");
            builder.Property(m => m.TipoCarroceria).IsRequired();
            builder.HasOne(m => m.Marca).WithMany().HasForeignKey(m => m.MarcaId).IsRequired();
        }
    }
    
    
}
