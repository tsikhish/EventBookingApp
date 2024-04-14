﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EventBookingApp.Migrations
{
    [DbContext(typeof(PersonContext))]
    partial class PersonContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Domain.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("Domain.CreateEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("EventName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaxBooking")
                        .HasColumnType("int");

                    b.Property<int?>("TickeetsId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TickeetsId");

                    b.ToTable("CreateEvent");
                });

            modelBuilder.Entity("Domain.Tickets", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("AppUserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("AppUserId1")
                        .HasColumnType("int");

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<string>("EventName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MaxBooking")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId1");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Domain.CreateEvent", b =>
                {
                    b.HasOne("Domain.Tickets", "Tickeets")
                        .WithMany("CreateEvents")
                        .HasForeignKey("TickeetsId");

                    b.Navigation("Tickeets");
                });

            modelBuilder.Entity("Domain.Tickets", b =>
                {
                    b.HasOne("Domain.AppUser", null)
                        .WithMany("Tickets")
                        .HasForeignKey("AppUserId1");
                });

            modelBuilder.Entity("Domain.AppUser", b =>
                {
                    b.Navigation("Tickets");
                });

            modelBuilder.Entity("Domain.Tickets", b =>
                {
                    b.Navigation("CreateEvents");
                });
#pragma warning restore 612, 618
        }
    }
}
