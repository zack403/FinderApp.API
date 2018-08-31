﻿// <auto-generated />
using System;
using FinderApp.API.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FinderApp.API.Migrations
{
    [DbContext(typeof(FinderDbContext))]
    [Migration("20180831154244_MadeUserEntityPropertiesRequired")]
    partial class MadeUserEntityPropertiesRequired
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-preview1-35029");

            modelBuilder.Entity("FinderApp.API.Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired();

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FinderApp.API.Model.Value", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Values");

                    b.HasData(
                        new { Id = 1, Name = "value 1" },
                        new { Id = 2, Name = "Value 2" },
                        new { Id = 3, Name = "Value 3" }
                    );
                });
#pragma warning restore 612, 618
        }
    }
}
