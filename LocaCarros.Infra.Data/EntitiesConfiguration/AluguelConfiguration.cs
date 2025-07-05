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
    public class AluguelConfiguration : IEntityTypeConfiguration<Aluguel>
    {
        public void Configure(EntityTypeBuilder<Aluguel> builder)
        {
          builder.HasKey(a => a.Id);
            builder.Property(a => a.DataInicio).IsRequired();
            builder.Property(a => a.DataDevolucao).IsRequired();
            builder.Property(a => a.ValorAluguel).IsRequired().HasColumnType("decimal(18,2)");
            
            builder.HasOne(a => a.Carro)
                   .WithMany(c => c.Alugueis)
                   .HasForeignKey(a => a.CarroId)
                   .IsRequired();
           
        }
    }
}
