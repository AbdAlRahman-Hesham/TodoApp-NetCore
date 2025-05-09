document.addEventListener("DOMContentLoaded", function () {
  // API Configuration
  const API_BASE_URL = "https://localhost:7184/api";
  let authToken = localStorage.getItem("auth_token") || "";

  // DOM Elements
  const authSection = document.getElementById("auth-section");
  const userSection = document.getElementById("user-section");
  const usernameDisplay = document.getElementById("username-display");
  const logoutBtn = document.getElementById("logout-btn");
  const mainContent = document.getElementById("main-content");
  const todoList = document.getElementById("todo-list");
  const loadingTodos = document.getElementById("loading-todos");
  const noTodos = document.getElementById("no-todos");

  // Login elements
  const loginForm = document.getElementById("login-form");
  const loginEmail = document.getElementById("login-email");
  const loginPassword = document.getElementById("login-password");
  const loginSubmit = document.getElementById("login-submit");
  const loginError = document.getElementById("login-error");

  // Register elements
  const registerForm = document.getElementById("register-form");
  const registerUsername = document.getElementById("register-username");
  const registerEmail = document.getElementById("register-email");
  const registerPassword = document.getElementById("register-password");
  const registerSubmit = document.getElementById("register-submit");
  const registerError = document.getElementById("register-error");

  // Todo elements
  const todoForm = document.getElementById("todo-form");
  const todoId = document.getElementById("todo-id");
  const todoTitle = document.getElementById("todo-title");
  const todoDescription = document.getElementById("todo-description");
  const todoPriority = document.getElementById("todo-priority");
  const todoDueDate = document.getElementById("todo-due-date");
  const todoSubmit = document.getElementById("todo-submit");
  const todoError = document.getElementById("todo-error");
  const addTodoBtn = document.getElementById("add-todo-btn");

  // Filter elements
  const searchInput = document.getElementById("search-input");
  const statusFilter = document.getElementById("status-filter");
  const priorityFilter = document.getElementById("priority-filter");
  const clearFilters = document.getElementById("clear-filters");

  // Delete confirmation
  const confirmDelete = document.getElementById("confirm-delete");
  let todoToDelete = null;

  // Modal instances
  const loginModal = new bootstrap.Modal(document.getElementById("loginModal"));
  const registerModal = new bootstrap.Modal(
    document.getElementById("registerModal")
  );
  const todoModal = new bootstrap.Modal(document.getElementById("todoModal"));
  const deleteModal = new bootstrap.Modal(
    document.getElementById("deleteModal")
  );

  // Initialize the app
  initApp();

  function initApp() {
    // Check if user is logged in
    if (authToken) {
      fetchUserInfo();
      loadTodos();
      showMainContent();
    } else {
      showAuthSection();
    }

    // Set up event listeners
    setupEventListeners();
  }

    function setupEventListeners() {
        document
          .getElementById("start-date")
          .addEventListener("change", loadTodos);
        document
          .getElementById("end-date")
          .addEventListener("change", loadTodos);
        document
          .getElementById("overdue-toggle")
          .addEventListener("change", loadTodos);
        document
          .getElementById("due-today-toggle")
          .addEventListener("change", loadTodos);
        document
          .getElementById("due-week-toggle")
          .addEventListener("change", loadTodos);
        document
          .getElementById("sort-by")
          .addEventListener("change", loadTodos);
        document
          .getElementById("sort-direction")
          .addEventListener("change", loadTodos);
    // Login
    loginSubmit.addEventListener("click", handleLogin);
    loginForm.addEventListener("submit", function (e) {
      e.preventDefault();
      handleLogin();
    });

    // Register
    registerSubmit.addEventListener("click", handleRegister);
    registerForm.addEventListener("submit", function (e) {
      e.preventDefault();
      handleRegister();
    });

    // Logout
    logoutBtn.addEventListener("click", handleLogout);

    // Todo form
    todoSubmit.addEventListener("click", handleTodoSubmit);
    todoForm.addEventListener("submit", function (e) {
      e.preventDefault();
      handleTodoSubmit();
    });

    // Add todo button
    addTodoBtn.addEventListener("click", () => {
      resetTodoForm();
      todoModal.show();
    });

    // Filters
    searchInput.addEventListener("input", loadTodos);
    statusFilter.addEventListener("change", loadTodos);
    priorityFilter.addEventListener("change", loadTodos);
    clearFilters.addEventListener("click", clearAllFilters);

    // Delete confirmation
    confirmDelete.addEventListener("click", () => {
      if (todoToDelete) {
        deleteTodo(todoToDelete);
        todoToDelete = null;
      }
      deleteModal.hide();
    });
  }

  function showAuthSection() {
    authSection.classList.remove("d-none");
    userSection.classList.add("d-none");
    mainContent.classList.add("d-none");
  }

  function showMainContent() {
    authSection.classList.add("d-none");
    userSection.classList.remove("d-none");
    mainContent.classList.remove("d-none");
  }

  // API Functions
  async function fetchUserInfo() {
    try {
      const response = await fetch(`${API_BASE_URL}/Account/me`, {
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      });

      if (response.ok) {
        const user = await response.json();
        usernameDisplay.textContent = user.userName;
      } else {
        handleLogout();
      }
    } catch (error) {
      console.error("Error fetching user info:", error);
      showError(loginError, "Failed to fetch user information");
    }
  }

  async function handleLogin() {
    const email = loginEmail.value.trim();
    const password = loginPassword.value.trim();

    if (!email || !password) {
      showError(loginError, "Please fill in all fields");
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/Account/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: email,
          password: password,
        }),
      });

      if (response.ok) {
        const data = await response.json();
        authToken = data.token;
        localStorage.setItem("auth_token", authToken);

        // Reset form and hide modal
        loginForm.reset();
        loginModal.hide();

        // Show main content
        fetchUserInfo();
        loadTodos();
        showMainContent();
      } else {
        const errorData = await response.json();
        showError(loginError, errorData.message || "Login failed");
      }
    } catch (error) {
      console.error("Login error:", error);
      showError(loginError, "Failed to connect to server");
    }
  }

  async function handleRegister() {
    const username = registerUsername.value.trim();
    const email = registerEmail.value.trim();
    const password = registerPassword.value.trim();

    if (!username || !email || !password) {
      showError(registerError, "Please fill in all fields");
      return;
    }

    // Simple password validation
    if (
      password.length < 8 ||
      !/[A-Z]/.test(password) ||
      !/[a-z]/.test(password) ||
      !/[0-9]/.test(password) ||
      !/[^A-Za-z0-9]/.test(password)
    ) {
      showError(
        registerError,
        "Password must be at least 8 characters with uppercase, lowercase, number and special character"
      );
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/Account/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          userName: username,
          email: email,
          password: password,
        }),
      });

      if (response.ok) {
        const data = await response.json();
        authToken = data.token;
        localStorage.setItem("auth_token", authToken);

        // Reset form and hide modal
        registerForm.reset();
        registerModal.hide();

        // Show main content
        fetchUserInfo();
        loadTodos();
        showMainContent();
      } else {
        const errorData = await response.json();
        let errorMessage = errorData.message || "Registration failed";

        // Handle validation errors
        if (errorData.errors) {
          const errorList = [];
          for (const [key, messages] of Object.entries(errorData.errors)) {
            errorList.push(...messages);
          }
          errorMessage = errorList.join("<br>");
        }

        showError(registerError, errorMessage);
      }
    } catch (error) {
      console.error("Registration error:", error);
      showError(registerError, "Failed to connect to server");
    }
  }

  function handleLogout() {
    authToken = "";
    localStorage.removeItem("auth_token");
    showAuthSection();
  }

  async function loadTodos() {
    loadingTodos.classList.remove("d-none");
    noTodos.classList.add("d-none");
    todoList.innerHTML = "";

    // Build query parameters
    const params = new URLSearchParams();

    // Add basic filters
    if (searchInput.value.trim()) {
      params.append("SearchTerm", searchInput.value.trim());
    }

    if (statusFilter.value !== "all") {
      params.append("Status", statusFilter.value);
    }

    if (priorityFilter.value !== "all") {
      params.append("Priority", priorityFilter.value);
    }

    // Add date range filters
    const startDate = document.getElementById("start-date").value;
    const endDate = document.getElementById("end-date").value;

    if (startDate) {
      params.append("StartDate", formatDateForAPI(startDate));
    }

    if (endDate) {
      params.append("EndDate", formatDateForAPI(endDate));
    }

    // Add toggle filters
    const overdueToggle = document.getElementById("overdue-toggle");
    const dueTodayToggle = document.getElementById("due-today-toggle");
    const dueWeekToggle = document.getElementById("due-week-toggle");

    if (overdueToggle.checked) {
      params.append("IsOverdue", "true");
    }

    if (dueTodayToggle.checked) {
      params.append("IsDueToday", "true");
    }

    if (dueWeekToggle.checked) {
      params.append("IsDueThisWeek", "true");
    }

    // Add sorting
    const sortBy = document.getElementById("sort-by").value;
    const sortDirection = document.getElementById("sort-direction").value;

    if (sortBy) {
      params.append("SortBy", sortBy);
      params.append("SortDescending", sortDirection);
    }

    // Always include pagination
    params.append("PageNumber", "1");
    params.append("PageSize", "10");

    try {
      const response = await fetch(
        `${API_BASE_URL}/Todo?${params.toString()}`,
        {
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        }
      );

      if (response.ok) {
        const todos = await response.json();
        renderTodos(todos);
      } else {
        throw new Error("Failed to fetch todos");
      }
    } catch (error) {
      console.error("Error loading todos:", error);
      todoList.innerHTML = `
            <div class="alert alert-danger">
                Failed to load todos. Please try again.
            </div>
        `;
    } finally {
      loadingTodos.classList.add("d-none");
    }
  }

  // Helper function to format dates for API (DD-MM-YYYY)
  function formatDateForAPI(dateString) {
    if (!dateString) return "";

    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const year = date.getFullYear();

    return `${month}-${day}-${year}`;
  }

  function renderTodos(todos) {
    todoList.innerHTML = "";

    if (todos.length === 0) {
      noTodos.classList.remove("d-none");
      return;
    }

    todos.forEach((todo) => {
      const todoItem = document.createElement("div");
      todoItem.className = `list-group-item list-group-item-action todo-item priority-${
        todo.priority
      } ${todo.status === 2 ? "completed" : ""}`;

      // Format due date
      const dueDate = todo.dueDate ? new Date(todo.dueDate) : null;
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      let dueDateText = "";
      if (dueDate) {
        const isOverdue = dueDate < today && todo.status !== 2;
        dueDateText = `
                    <span class="due-date ${isOverdue ? "overdue" : ""}">
                        <i class="far fa-calendar-alt me-1"></i>
                        Due: ${dueDate.toLocaleDateString()}
                        ${isOverdue ? "(Overdue)" : ""}
                    </span>
                `;
      }

      // Status badge
      const statusText =
        ["Not Started", "In Progress", "Completed"][todo.status] || "Unknown";
      const statusClass =
        ["secondary", "primary", "success"][todo.status] || "secondary";

      // Priority badge
      const priorityText =
        ["Low", "Medium", "High"][todo.priority] || "Unknown";
      const priorityClass =
        ["success", "warning", "danger"][todo.priority] || "secondary";

      todoItem.innerHTML = `
                <div class="d-flex w-100 justify-content-between align-items-center">
                    <div>
                        <h6 class="mb-1 todo-title">${todo.title}</h6>
                        <p class="mb-1">${
                          todo.description || "No description"
                        }</p>
                        ${dueDateText}
                    </div>
                    <div class="d-flex align-items-center">
                        <span class="status-badge badge bg-${statusClass} me-2">${statusText}</span>
                        <span class="priority-badge badge bg-${priorityClass} me-2">${priorityText}</span>
                        <div class="todo-actions btn-group">
                            <button class="btn btn-sm btn-outline-primary edit-todo" data-id="${
                              todo.id
                            }">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-sm btn-outline-danger delete-todo" data-id="${
                              todo.id
                            }">
                                <i class="fas fa-trash"></i>
                            </button>
                            ${
                              todo.status !== 2
                                ? `
                                <button class="btn btn-sm btn-outline-success complete-todo" data-id="${todo.id}">
                                    <i class="fas fa-check"></i>
                                </button>
                            `
                                : ""
                            }
                        </div>
                    </div>
                </div>
            `;

      todoList.appendChild(todoItem);
    });

    // Add event listeners to action buttons
    document.querySelectorAll(".edit-todo").forEach((btn) => {
      btn.addEventListener("click", () => editTodo(btn.dataset.id));
    });

    document.querySelectorAll(".delete-todo").forEach((btn) => {
      btn.addEventListener("click", () => {
        todoToDelete = btn.dataset.id;
        deleteModal.show();
      });
    });

    document.querySelectorAll(".complete-todo").forEach((btn) => {
      btn.addEventListener("click", () => completeTodo(btn.dataset.id));
    });
  }

  function resetTodoForm() {
    todoForm.reset();
    todoId.value = "";
    document.getElementById("todoModalLabel").textContent = "Add New Todo";
  }

  async function editTodo(id) {
    try {
      const response = await fetch(`${API_BASE_URL}/Todo/${id}`, {
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      });

      if (response.ok) {
        const todo = await response.json();

        // Fill the form
        todoId.value = todo.id;
        todoTitle.value = todo.title;
        todoDescription.value = todo.description || "";
        todoPriority.value = todo.priority;

        if (todo.dueDate) {
          const dueDate = new Date(todo.dueDate);
          todoDueDate.value = dueDate.toISOString().split("T")[0];
        } else {
          todoDueDate.value = "";
        }

        document.getElementById("todoModalLabel").textContent = "Edit Todo";
        todoModal.show();
      } else {
        throw new Error("Failed to fetch todo");
      }
    } catch (error) {
      console.error("Error editing todo:", error);
      showError(todoError, "Failed to load todo for editing");
    }
  }

  async function handleTodoSubmit() {
    const id = todoId.value;
    const title = todoTitle.value.trim();
    const description = todoDescription.value.trim();
    const priority = todoPriority.value;
    const dueDate = todoDueDate.value;

    if (!title) {
      showError(todoError, "Title is required");
      return;
    }

    const todoData = {
      title: title,
      description: description,
      priority: parseInt(priority),
      dueDate: dueDate || null,
    };

    // For updates, include the status and id
    if (id) {
      todoData.id = id;

      // Get current status (we don't want to change it on update)
      try {
        const response = await fetch(`${API_BASE_URL}/Todo/${id}`, {
          headers: {
            Authorization: `Bearer ${authToken}`,
          },
        });

        if (response.ok) {
          const currentTodo = await response.json();
          todoData.status = currentTodo.status;
        }
      } catch (error) {
        console.error("Error fetching current todo:", error);
      }
    } else {
      todoData.status = 0; // Default to "Not Started"
    }

    try {
      const url = id ? `${API_BASE_URL}/Todo` : `${API_BASE_URL}/Todo`;
      const method = id ? "PUT" : "POST";

      const response = await fetch(url, {
        method: method,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${authToken}`,
        },
        body: JSON.stringify(todoData),
      });

      if (response.ok) {
        const todo = await response.json();
        todoModal.hide();
        loadTodos();
      } else {
        const errorData = await response.json();
        showError(todoError, errorData.message || "Failed to save todo");
      }
    } catch (error) {
      console.error("Error saving todo:", error);
      showError(todoError, "Failed to connect to server");
    }
  }

  async function completeTodo(id) {
    try {
      const response = await fetch(`${API_BASE_URL}/Todo/${id}/complete`, {
        method: "PATCH",
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      });

      if (response.ok) {
        loadTodos();
      } else {
        throw new Error("Failed to complete todo");
      }
    } catch (error) {
      console.error("Error completing todo:", error);
      alert("Failed to mark todo as complete");
    }
  }

  async function deleteTodo(id) {
    try {
      const response = await fetch(`${API_BASE_URL}/Todo/${id}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      });

      if (response.ok) {
        loadTodos();
      } else {
        throw new Error("Failed to delete todo");
      }
    } catch (error) {
      console.error("Error deleting todo:", error);
      alert("Failed to delete todo");
    }
  }

  function clearAllFilters() {
    searchInput.value = "";
    statusFilter.value = "all";
    priorityFilter.value = "all";
    document.getElementById("start-date").value = "";
    document.getElementById("end-date").value = "";
    document.getElementById("overdue-toggle").checked = false;
    document.getElementById("due-today-toggle").checked = false;
    document.getElementById("due-week-toggle").checked = false;
    document.getElementById("sort-by").value = "";
    document.getElementById("sort-direction").value = "false";
    loadTodos();
  }

  function showError(element, message) {
    element.innerHTML = message;
    element.classList.remove("d-none");

    setTimeout(() => {
      element.classList.add("d-none");
    }, 5000);
  }
});
