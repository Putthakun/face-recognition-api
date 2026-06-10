using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace face_recognition_api.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRoles_Employees_EmployeeEmpId",
                table: "EmployeeRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceEmbeddeds_Employees_EmployeeEmpId",
                table: "FaceEmbeddeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Employees_EmployeeEmpId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_FaceEmbeddeds_EmployeeEmpId",
                table: "FaceEmbeddeds");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeRoles_EmployeeEmpId",
                table: "EmployeeRoles");

            migrationBuilder.DropColumn(
                name: "EmployeeEmpId",
                table: "FaceEmbeddeds");

            migrationBuilder.DropColumn(
                name: "EmployeeEmpId",
                table: "EmployeeRoles");

            migrationBuilder.RenameColumn(
                name: "EmployeeEmpId",
                table: "Transactions",
                newName: "CameraId1");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_EmployeeEmpId",
                table: "Transactions",
                newName: "IX_Transactions_CameraId1");

            migrationBuilder.CreateIndex(
                name: "IX_FaceEmbeddeds_EmpId",
                table: "FaceEmbeddeds",
                column: "EmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRoles_Employees_EmpId",
                table: "EmployeeRoles",
                column: "EmpId",
                principalTable: "Employees",
                principalColumn: "EmpId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceEmbeddeds_Employees_EmpId",
                table: "FaceEmbeddeds",
                column: "EmpId",
                principalTable: "Employees",
                principalColumn: "EmpId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Cameras_CameraId1",
                table: "Transactions",
                column: "CameraId1",
                principalTable: "Cameras",
                principalColumn: "CameraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Employees_EmpId",
                table: "Transactions",
                column: "EmpId",
                principalTable: "Employees",
                principalColumn: "EmpId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeRoles_Employees_EmpId",
                table: "EmployeeRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_FaceEmbeddeds_Employees_EmpId",
                table: "FaceEmbeddeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Cameras_CameraId1",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Employees_EmpId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_FaceEmbeddeds_EmpId",
                table: "FaceEmbeddeds");

            migrationBuilder.RenameColumn(
                name: "CameraId1",
                table: "Transactions",
                newName: "EmployeeEmpId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_CameraId1",
                table: "Transactions",
                newName: "IX_Transactions_EmployeeEmpId");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeEmpId",
                table: "FaceEmbeddeds",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EmployeeEmpId",
                table: "EmployeeRoles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_FaceEmbeddeds_EmployeeEmpId",
                table: "FaceEmbeddeds",
                column: "EmployeeEmpId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoles_EmployeeEmpId",
                table: "EmployeeRoles",
                column: "EmployeeEmpId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeRoles_Employees_EmployeeEmpId",
                table: "EmployeeRoles",
                column: "EmployeeEmpId",
                principalTable: "Employees",
                principalColumn: "EmpId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FaceEmbeddeds_Employees_EmployeeEmpId",
                table: "FaceEmbeddeds",
                column: "EmployeeEmpId",
                principalTable: "Employees",
                principalColumn: "EmpId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Employees_EmployeeEmpId",
                table: "Transactions",
                column: "EmployeeEmpId",
                principalTable: "Employees",
                principalColumn: "EmpId");
        }
    }
}
