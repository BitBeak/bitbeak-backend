﻿// <auto-generated />
using System;
using BitBeakAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BitBeakAPI.Migrations
{
    [DbContext(typeof(BitBeakContext))]
    [Migration("20240521173032_nivelname")]
    partial class nivelname
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BitBeakAPI.Models.Lacuna", b =>
                {
                    b.Property<int>("IdLacuna")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdLacuna"));

                    b.Property<string>("ColunaA")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ColunaB")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IdQuestao")
                        .HasColumnType("int");

                    b.HasKey("IdLacuna");

                    b.HasIndex("IdQuestao");

                    b.ToTable("Lacunas");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelNivelTrilha", b =>
                {
                    b.Property<int>("IdNivel")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdNivel"));

                    b.Property<int>("IdTrilha")
                        .HasColumnType("int");

                    b.Property<int>("Nivel")
                        .HasColumnType("int");

                    b.Property<string>("NivelName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdNivel");

                    b.HasIndex("IdTrilha");

                    b.ToTable("NiveisTrilha");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelQuestao", b =>
                {
                    b.Property<int>("IdQuestao")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdQuestao"));

                    b.Property<string>("Enunciado")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("IdNivel")
                        .HasColumnType("int");

                    b.Property<string>("SolucaoEsperada")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Tipo")
                        .HasColumnType("int");

                    b.HasKey("IdQuestao");

                    b.HasIndex("IdNivel");

                    b.ToTable("Questoes");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelTrilha", b =>
                {
                    b.Property<int>("IdTrilha")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdTrilha"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("IdTrilha");

                    b.ToTable("Trilhas");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelUsuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuario"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Experiencia")
                        .HasColumnType("int");

                    b.Property<int>("Nivel")
                        .HasColumnType("int");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PasswordResetTokenExpiry")
                        .HasColumnType("datetime2");

                    b.Property<int>("Penas")
                        .HasColumnType("int");

                    b.Property<string>("SenhaCriptografada")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdUsuario");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelUsuarioTrilhaProgresso", b =>
                {
                    b.Property<int>("IdProgresso")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdProgresso"));

                    b.Property<int>("Experiencia")
                        .HasColumnType("int");

                    b.Property<int>("IdTrilha")
                        .HasColumnType("int");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.Property<int>("Nivel")
                        .HasColumnType("int");

                    b.HasKey("IdProgresso");

                    b.HasIndex("IdTrilha");

                    b.HasIndex("IdUsuario");

                    b.ToTable("ModelUsuarioTrilhaProgresso");
                });

            modelBuilder.Entity("BitBeakAPI.Models.OpcaoResposta", b =>
                {
                    b.Property<int>("IdOpcao")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdOpcao"));

                    b.Property<bool>("Correta")
                        .HasColumnType("bit");

                    b.Property<int>("IdQuestao")
                        .HasColumnType("int");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdOpcao");

                    b.HasIndex("IdQuestao");

                    b.ToTable("OpcoesResposta");
                });

            modelBuilder.Entity("BitBeakAPI.Models.Lacuna", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelQuestao", "Questao")
                        .WithMany("Lacunas")
                        .HasForeignKey("IdQuestao")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Questao");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelNivelTrilha", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelTrilha", "Trilha")
                        .WithMany("Niveis")
                        .HasForeignKey("IdTrilha")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trilha");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelQuestao", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelNivelTrilha", "Nivel")
                        .WithMany("Questoes")
                        .HasForeignKey("IdNivel");

                    b.Navigation("Nivel");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelUsuarioTrilhaProgresso", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelTrilha", "Trilha")
                        .WithMany()
                        .HasForeignKey("IdTrilha")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BitBeakAPI.Models.ModelUsuario", "Usuario")
                        .WithMany("TrilhasProgresso")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Trilha");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("BitBeakAPI.Models.OpcaoResposta", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelQuestao", "Questao")
                        .WithMany("Opcoes")
                        .HasForeignKey("IdQuestao")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Questao");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelNivelTrilha", b =>
                {
                    b.Navigation("Questoes");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelQuestao", b =>
                {
                    b.Navigation("Lacunas");

                    b.Navigation("Opcoes");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelTrilha", b =>
                {
                    b.Navigation("Niveis");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelUsuario", b =>
                {
                    b.Navigation("TrilhasProgresso");
                });
#pragma warning restore 612, 618
        }
    }
}
