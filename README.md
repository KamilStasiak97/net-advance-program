# Advanced .NET Program - Cart & Catalog Services

A demonstration of layered architecture and Clean Architecture principles in .NET with C++.

## Project Structure

### Cart Service (Week 1)
Implements **layered architecture** with logical separation (folders/namespaces).
- **Database**: LiteDB (NoSQL, file-based: `Data/cart.db`)
- **Architecture**: BLL + DAL pattern
- **Separation**: Logical (folders within single project)

### Catalog Service (Week 2)
Implements **Clean Architecture** with physical separation (separate DLLs).
- **Database**: SQLite (SQL, file-based: `Data/catalog.db`)
- **Architecture**: Domain → Application → Infrastructure
- **Separation**: Physical (separate class libraries)

## Solution Structure