﻿// <auto-generated />
using ReversePlace;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ReversePlace.Migrations
{
    [DbContext(typeof(PlaceGeocoderContext))]
    [Migration("20220305062725_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.2");

            modelBuilder.Entity("GeoSqlite.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CountryCode")
                        .HasColumnType("TEXT");

                    b.Property<int>("Latitude")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Longitude")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });
#pragma warning restore 612, 618
        }
    }
}