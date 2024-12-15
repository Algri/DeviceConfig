﻿// <auto-generated />
using IoT.RPiController.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IoT.RPiController.Data.Migrations
{
    [DbContext(typeof(RPiContext))]
    [Migration("20220128173253_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("IoT.RPiController.Data.Entities.Config", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte>("Address")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("PortA")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("PortB")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Configs");
                });
#pragma warning restore 612, 618
        }
    }
}
