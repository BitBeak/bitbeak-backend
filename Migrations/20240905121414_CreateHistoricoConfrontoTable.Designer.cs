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
    [Migration("20240905121414_CreateHistoricoConfrontoTable")]
    partial class CreateHistoricoConfrontoTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BitBeakAPI.Models.CodeFill", b =>
                {
                    b.Property<int>("IdCodeFill")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCodeFill"));

                    b.Property<int>("IdQuestao")
                        .HasColumnType("int");

                    b.Property<string>("RespostaEsperada")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdCodeFill");

                    b.HasIndex("IdQuestao");

                    b.ToTable("CodeFill");
                });

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

            modelBuilder.Entity("BitBeakAPI.Models.ModelNivelUsuario", b =>
                {
                    b.Property<int>("IdNivelUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdNivelUsuario"));

                    b.Property<int>("ExperienciaNecessaria")
                        .HasColumnType("int");

                    b.Property<int>("NivelUsuario")
                        .HasColumnType("int");

                    b.HasKey("IdNivelUsuario");

                    b.ToTable("NiveisUsuario");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelQuestao", b =>
                {
                    b.Property<int>("IdQuestao")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdQuestao"));

                    b.Property<string>("Codigo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Enunciado")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("IdNivel")
                        .HasColumnType("int");

                    b.Property<string>("Instrucoes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.Property<string>("CodigoDeAmizade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ExperienciaQuinzenalUsuario")
                        .HasColumnType("int");

                    b.Property<int>("ExperienciaUsuario")
                        .HasColumnType("int");

                    b.Property<int>("NivelUsuario")
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

            modelBuilder.Entity("BitBeakAPI.Models.ModelUsuarioNivelConcluido", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DataConclusao")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdNivel")
                        .HasColumnType("int");

                    b.Property<int>("IdTrilha")
                        .HasColumnType("int");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UsuarioNiveisConcluidos");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelUsuarioTrilhaConcluida", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DataConclusao")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdTrilha")
                        .HasColumnType("int");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UsuarioTrilhasConcluidas");
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

            modelBuilder.Entity("ModelAmizade", b =>
                {
                    b.Property<int>("IdAmizade")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdAmizade"));

                    b.Property<int>("IdAmigo")
                        .HasColumnType("int");

                    b.Property<int>("IdUsuario")
                        .HasColumnType("int");

                    b.HasKey("IdAmizade");

                    b.HasIndex("IdAmigo");

                    b.HasIndex("IdUsuario");

                    b.ToTable("Amizades");
                });

            modelBuilder.Entity("ModelDesafio", b =>
                {
                    b.Property<int>("IdDesafio")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDesafio"));

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<bool>("DesafianteJogando")
                        .HasColumnType("bit");

                    b.Property<bool>("Finalizado")
                        .HasColumnType("bit");

                    b.Property<int>("IdDesafiado")
                        .HasColumnType("int");

                    b.Property<int>("IdDesafiante")
                        .HasColumnType("int");

                    b.Property<int>("IdTrilha")
                        .HasColumnType("int");

                    b.Property<int>("InsigniasDesafiado")
                        .HasColumnType("int");

                    b.Property<int>("InsigniasDesafiante")
                        .HasColumnType("int");

                    b.Property<int>("NivelDesafiado")
                        .HasColumnType("int");

                    b.Property<int>("NivelDesafiante")
                        .HasColumnType("int");

                    b.Property<DateTime>("UltimaAtualizacao")
                        .HasColumnType("datetime2");

                    b.HasKey("IdDesafio");

                    b.HasIndex("IdDesafiado");

                    b.HasIndex("IdDesafiante");

                    b.HasIndex("IdTrilha");

                    b.ToTable("Desafios");
                });

            modelBuilder.Entity("BitBeakAPI.Models.CodeFill", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelQuestao", "Questao")
                        .WithMany("CodeFill")
                        .HasForeignKey("IdQuestao")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Questao");
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

            modelBuilder.Entity("BitBeakAPI.Models.OpcaoResposta", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelQuestao", "Questao")
                        .WithMany("Opcoes")
                        .HasForeignKey("IdQuestao")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Questao");
                });

            modelBuilder.Entity("ModelAmizade", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelUsuario", "Amigo")
                        .WithMany()
                        .HasForeignKey("IdAmigo")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BitBeakAPI.Models.ModelUsuario", "Usuario")
                        .WithMany("Amigos")
                        .HasForeignKey("IdUsuario")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Amigo");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("ModelDesafio", b =>
                {
                    b.HasOne("BitBeakAPI.Models.ModelUsuario", "Desafiado")
                        .WithMany()
                        .HasForeignKey("IdDesafiado")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BitBeakAPI.Models.ModelUsuario", "Desafiante")
                        .WithMany()
                        .HasForeignKey("IdDesafiante")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BitBeakAPI.Models.ModelTrilha", "Trilha")
                        .WithMany()
                        .HasForeignKey("IdTrilha")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Desafiado");

                    b.Navigation("Desafiante");

                    b.Navigation("Trilha");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelNivelTrilha", b =>
                {
                    b.Navigation("Questoes");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelQuestao", b =>
                {
                    b.Navigation("CodeFill");

                    b.Navigation("Lacunas");

                    b.Navigation("Opcoes");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelTrilha", b =>
                {
                    b.Navigation("Niveis");
                });

            modelBuilder.Entity("BitBeakAPI.Models.ModelUsuario", b =>
                {
                    b.Navigation("Amigos");
                });
#pragma warning restore 612, 618
        }
    }
}
