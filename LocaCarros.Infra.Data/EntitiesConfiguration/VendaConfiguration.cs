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
    public class VendaConfiguration : IEntityTypeConfiguration<Venda>
    {
        public void Configure(EntityTypeBuilder<Venda> builder)
        {
         
            builder.HasKey(v => v.Id);
            builder.Property(v => v.DataVenda).IsRequired();
            builder.Property(v => v.ValorVenda).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.RowVersion)
             .IsRowVersion()
             .IsConcurrencyToken();
            builder.HasOne(v => v.Carro)
                   .WithMany()
                   .HasForeignKey(v => v.CarroId)
                   .IsRequired();

        }
    }
}
