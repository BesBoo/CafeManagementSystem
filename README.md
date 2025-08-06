# 🏪 Cafe Management System

A comprehensive Windows Forms application built with C# for managing cafe operations including order management, inventory tracking, staff management, and billing.

![Cafe Management System](https://img.shields.io/badge/Platform-.NET%20Framework-blue)
![Language](https://img.shields.io/badge/Language-C%23-green)
![Database](https://img.shields.io/badge/Database-SQL%20Server-red)

## 📋 Table of Contents

- [Features](#features)
- [System Requirements](#system-requirements)
- [Installation](#installation)
- [Database Setup](#database-setup)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Key Components](#key-components)
- [Screenshots](#screenshots)
- [Contributing](#contributing)

## ✨ Features

### 🛍️ Order Management
- **Multiple Order Types**: Dine-in, Take Away, and Delivery
- **Real-time Order Processing**: Kitchen Order Tickets (KOT) system
- **Order Status Tracking**: Pending, Complete, Hold, Paid
- **Table Management**: Dynamic table selection and availability tracking

### 📦 Inventory Management
- **Product Management**: Add, edit, delete products with categories
- **Stock Tracking**: Real-time inventory updates
- **Category Management**: Organize products by categories
- **Image Support**: Product images for better visualization

### 👥 Staff Management
- **Role-based Access**: Cashier, Waiter, Driver, Manager roles
- **Staff Information**: Complete staff profile management
- **Driver Assignment**: Automatic driver assignment for delivery orders

### 💰 Billing & Payments
- **Invoice Generation**: Automatic bill generation
- **Excel Export**: Order details exported to Excel format
- **Payment Processing**: Cash handling with change calculation
- **Bill History**: Complete transaction records

### 🎨 User Interface
- **Theme Support**: Light and Dark mode themes
- **Responsive Design**: Adaptive UI components
- **User-friendly**: Intuitive navigation and controls

## 🔧 System Requirements

- **Operating System**: Windows 7/8/10/11
- **.NET Framework**: 4.7.2 or higher
- **Database**: SQL Server 2014 or higher / SQL Server Express
- **Libraries**: 
  - OfficeOpenXml (EPPlus)
  - System.Data.SqlClient

## 🚀 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-username/cafe-management-system.git
   cd cafe-management-system
   ```

2. **Open the project**
   - Open Visual Studio
   - File → Open → Project/Solution
   - Select the `.sln` file

3. **Install NuGet packages**
   ```powershell
   Install-Package EPPlus
   Install-Package System.Data.SqlClient
   ```

4. **Update connection string**
   In `MainClass.cs`, update the connection string:
   ```csharp
   public static readonly string con_string = @"Data Source=YOUR_SERVER;Initial Catalog=projectLTCSDL;Integrated Security=True;";
   ```

## 🗄️ Database Setup

### Required Tables

```sql
-- Users table
CREATE TABLE users (
    userID int IDENTITY(1,1) PRIMARY KEY,
    uName varchar(50),
    username varchar(50) UNIQUE,
    upass varchar(50),
    uRole varchar(20),
    isDisabled bit DEFAULT 0,
    failedAttempts int DEFAULT 0
);

-- Category table
CREATE TABLE category (
    catID int IDENTITY(1,1) PRIMARY KEY,
    catName varchar(50) UNIQUE
);

-- Products table
CREATE TABLE products (
    pID int IDENTITY(1,1) PRIMARY KEY,
    pName varchar(100),
    pPrice decimal(10,2),
    CategoryID int FOREIGN KEY REFERENCES category(catID),
    pImage varbinary(max),
    pStock int DEFAULT 0
);

-- Staff table
CREATE TABLE staff (
    staffID int IDENTITY(1,1) PRIMARY KEY,
    sName varchar(100),
    sPhone varchar(15),
    sRole varchar(20),
    sAddress varchar(200)
);

-- Tables table
CREATE TABLE tables (
    tID int IDENTITY(1,1) PRIMARY KEY,
    tName varchar(20),
    isAvailable bit DEFAULT 1
);

-- Main orders table
CREATE TABLE tblMain (
    MainID int IDENTITY(1,1) PRIMARY KEY,
    aDate date,
    aTime varchar(10),
    TableName varchar(20),
    WaiterName varchar(100),
    status varchar(20),
    orderType varchar(20),
    total decimal(10,2),
    received decimal(10,2),
    change decimal(10,2),
    driverID int,
    CustName varchar(100),
    CustPhone varchar(15)
);

-- Order details table
CREATE TABLE tblDetails (
    DetailID int IDENTITY(1,1) PRIMARY KEY,
    MainID int FOREIGN KEY REFERENCES tblMain(MainID),
    proID int FOREIGN KEY REFERENCES products(pID),
    qty int,
    price decimal(10,2),
    amount decimal(10,2)
);
```

## 🎯 Usage

### Login System
```csharp
// User authentication with failed attempt tracking
public static bool isvalidUser(string user, string pass)
{
    // Validates user credentials
    // Tracks failed login attempts
    // Disables account after 3 failed attempts
}
```

### Order Management
```csharp
// Creating a new order
private void btn_KOT_Click(object sender, EventArgs e)
{
    // Validates order items
    // Checks inventory availability
    // Updates stock levels
    // Creates order records
}
```

### Theme Management
```csharp
// Applying themes
MainClass.ApplyTheme(this, ThemeManager.CurrentTheme);
ThemeManager.ThemeChanged += OnThemeChanged;
```

## 📁 Project Structure

```
CafeManagement/
├── Model/
│   ├── frmAddCustomer.cs      # Customer information form
│   ├── frmBillList.cs         # Bill listing and selection
│   ├── frmCategoryAdd.cs      # Category management
│   ├── frmCheckout.cs         # Payment processing
│   ├── frmPOS.cs             # Main POS interface
│   ├── frmProductAdd.cs       # Product management
│   ├── frmStaffAdd.cs         # Staff management
│   ├── frmWaiterSelect.cs     # Waiter selection
│   └── ucProduct.cs           # Product user control
├── View/
│   └── frmProductView.cs      # Product listing view
├── MainClass.cs               # Core functionality and database operations
└── ThemeManager.cs            # Theme management system
```

## 🔑 Key Components

### 1. Point of Sale (POS) System
- **Dynamic Product Loading**: Products loaded from database with categories
- **Order Processing**: Real-time order creation and modification
- **Multiple Payment Options**: Cash handling with change calculation

### 2. Inventory Management
- **Stock Control**: Automatic stock deduction on orders
- **Product Categories**: Organized product management
- **Image Management**: Product photos for better identification

### 3. User Management
- **Role-based Access Control**: Different permissions for different roles
- **Security Features**: Account lockout after failed attempts
- **User Activity Tracking**: Login monitoring and session management

### 4. Reporting System
- **Excel Export**: Detailed order reports
- **Bill Generation**: Professional invoice creation
- **Sales Analytics**: Order history and tracking

## 🖼️ Screenshots

### Main POS Interface
The main POS interface features:
- Category-based product selection
- Shopping cart functionality
- Order type selection (Dine-in, Take Away, Delivery)
- Real-time total calculation

### Product Management
- Add/Edit products with images
- Category assignment
- Stock level management
- Price configuration

### Order Processing
- Kitchen Order Ticket generation
- Order status tracking
- Payment processing
- Receipt generation

## 🛠️ Configuration

### Database Connection
Update the connection string in `MainClass.cs`:
```csharp
public static readonly string con_string = @"Data Source=SERVER_NAME;Initial Catalog=DATABASE_NAME;Integrated Security=True;";
```

### Theme Configuration
The system supports two themes:
- **Light Theme**: Clean, bright interface
- **Dark Theme**: Modern, dark interface

### Export Settings
Excel files are saved to:
```csharp
string folderPath = @"D:\Hóa đơn";
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🙏 Acknowledgments

- Built with Windows Forms and C#
- Uses EPPlus for Excel generation
- SQL Server for data management
- Modern UI design principles

## 📞 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

---

**Made with ❤️ for the cafe industry**
