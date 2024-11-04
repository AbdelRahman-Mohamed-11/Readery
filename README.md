Redery is an e-commerce application  built using ASP.NET Core 8 with a modern n-tier architecture. It includes structured roles and permissions for managing and purchasing products, 
as well as robust integrations for payments and user authentication.

Key Features:
Roles and Permissions:
  Admin: Can add products, categories, and users.
  Company Users: Can purchase products and purchase within a thirty-day period

Payment Options:
  Company users can pay within a thirty-day period.
  regular users are required to make immediate payments.

Authentication:
  Users can log in using Google or Facebook accounts.
  Password reset functionality is available for users.

Payment Integration:
  Integrated with Stripe for payment processing.
  PayPal integration is available but currently inactive due to issues in Egypt. It can be enabled by uncommitting the PayPal-related code.

Technical Details:
  Unit of Work Pattern and Generic Repository: Used for managing data transactions and improving code reusability.
  Pagination and Filtering: Implemented using IQueryable with custom extension methods, ensuring efficient data handling and improved performance.
  IQueryable Extensions: Provides efficient handling of pagination and filtering through custom extension methods.
