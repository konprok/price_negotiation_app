﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PriceNegotiationApp.Database.DbContext;

#nullable disable

namespace PriceNegotiationApp.Migrations
{
    [DbContext(typeof(UserDbContext))]
    partial class UserDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.NegotiationEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("negotiation_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uuid")
                        .HasColumnName("client_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<decimal?>("FinalPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)")
                        .HasColumnName("final_price");

                    b.Property<bool>("Finished")
                        .HasColumnType("boolean")
                        .HasColumnName("finished");

                    b.Property<DateTime?>("ModyfiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modified_at");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<long>("ProductId")
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    b.HasKey("Id")
                        .HasName("PK_negotiation_entity");

                    b.HasIndex("OwnerId");

                    b.HasIndex("ProductId");

                    b.ToTable("negotiation_entity", (string)null);
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.ProductEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("product_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("BasePrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)")
                        .HasColumnName("base_price");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("description");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("modified_at");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.HasKey("Id")
                        .HasName("PK_product_entity");

                    b.HasIndex("OwnerId");

                    b.ToTable("product_entity", (string)null);
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.PropositionEntity", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("proposition_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTimeOffset?>("DecidedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("decided_at");

                    b.Property<bool?>("Decision")
                        .HasColumnType("boolean")
                        .HasColumnName("decision");

                    b.Property<long>("NegotiationId")
                        .HasColumnType("bigint")
                        .HasColumnName("negotiation_id");

                    b.Property<DateTimeOffset>("ProposedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("proposed_at");

                    b.Property<decimal>("ProposedPrice")
                        .HasPrecision(18, 2)
                        .HasColumnType("numeric(18,2)")
                        .HasColumnName("proposed_price");

                    b.HasKey("Id")
                        .HasName("PK_proposition_entity");

                    b.HasIndex("NegotiationId");

                    b.ToTable("proposition_entity", (string)null);
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("user_id");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("email");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)")
                        .HasColumnName("password_hash");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("PK_user_entity");

                    b.ToTable("user_entity", (string)null);
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.NegotiationEntity", b =>
                {
                    b.HasOne("PriceNegotiationApp.Database.Entities.UserEntity", "User")
                        .WithMany("Negotiations")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_negotiation_entity_user");

                    b.HasOne("PriceNegotiationApp.Database.Entities.ProductEntity", "Product")
                        .WithMany("Negotiations")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_negotiation_entity_product");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.ProductEntity", b =>
                {
                    b.HasOne("PriceNegotiationApp.Database.Entities.UserEntity", "CreatedBy")
                        .WithMany("CreatedProducts")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_product_entity_user");

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.PropositionEntity", b =>
                {
                    b.HasOne("PriceNegotiationApp.Database.Entities.NegotiationEntity", "Negotiation")
                        .WithMany("Proposition")
                        .HasForeignKey("NegotiationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_proposition_entity_negotiation");

                    b.Navigation("Negotiation");
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.NegotiationEntity", b =>
                {
                    b.Navigation("Proposition");
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.ProductEntity", b =>
                {
                    b.Navigation("Negotiations");
                });

            modelBuilder.Entity("PriceNegotiationApp.Database.Entities.UserEntity", b =>
                {
                    b.Navigation("CreatedProducts");

                    b.Navigation("Negotiations");
                });
#pragma warning restore 612, 618
        }
    }
}
