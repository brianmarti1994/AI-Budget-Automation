# 💸 AI Budget Automation (Crete Project)

> **AI-powered financial budget management** built with **.NET 8**, **Blazor WebAssembly**, **SQLite**, and **Ollama (LLM)**.  
> Manage budgets using **natural language commands** instead of forms.

---

## 🚀 What is this project?

**AI Budget Automation** allows users to create, update, and delete budget categories using plain English prompts like:

- 🧠 *“Create a category called Tools with a budget of 4000 for 2026”*
- ✏️ *“Update Tools budget for October to 200”*
- 🗑️ *“Delete Tools category”*
- ➗ *“Update Testers & Tools budget to 100/2 for October”*

The AI understands intent, categories, months, years, and even **math expressions**, then safely applies changes to the database.

---

## ✨ Key Features

### 🤖 AI-Driven Budget Management
- Natural language CRUD operations
- Supports **math expressions** (`100/2`, `4000/12`)
- Handles **multiple categories in one command**
- Safe delete with confirmation

### 📊 Professional Budget Grid
- Pivot view (Categories × Jan–Dec + Total)
- Sticky headers & category column
- Excel-like readability
- Auto refresh after AI actions

### 🧱 Clean Architecture
- Domain-Driven Design (DDD)
- Clear separation of concerns
- AI logic isolated in Infrastructure layer

### ⚡ Modern Stack
- .NET 8
- Blazor WebAssembly
- EF Core + SQLite
- Ollama (local LLM – no cloud dependency)

---

## 🏗️ Architecture Overview

```text
/src
 ├── Domain        → Entities & business rules
 ├── Application   → Use cases & orchestration
 ├── Infrastructure→ EF Core + Ollama AI
 ├── API          → REST API
 └── UI          → Blazor WebAssembly UI
