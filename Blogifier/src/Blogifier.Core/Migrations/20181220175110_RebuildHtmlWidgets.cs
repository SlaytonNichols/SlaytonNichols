﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Blogifier.Core.Migrations
{
    public partial class RebuildHtmlWidgets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HtmlWidgets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                               SqlServerValueGenerationStrategy.IdentityColumn)
                        .Annotation("MySql:ValueGenerationStrategy", 
                                MySqlValueGenerationStrategy.IdentityColumn)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                                NpgsqlValueGenerationStrategy.SerialColumn),
                    Name = table.Column<string>(nullable: true),
                    Theme = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtmlWidgets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HtmlWidgets");
        }
    }
}
