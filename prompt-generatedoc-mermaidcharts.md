## GitHub Copilot Agent Prompt: Aspire Solution Analysis & Documentation

1. **Launch the current solution**
   - If the application requires a token, ask the user to perform the login before proceeding.

2. **Analyze the entire solution** (all projects, code, and configuration files in the `Skinet` folder) to understand the overall goal and architecture.

3. **Analyze the different projects** to generate an architecture diagram representing the solution's structure and service relationships.
   - **Before generating the architecture diagram, ask the user to choose the preferred format:**
     - ASCII diagram (text-based, see example below)
     - Mermaid chart (image, generated using the `mcp-mermaid` server)
     - Both (generate and include both diagram types)
   - Generate the diagram(s) in the format(s) selected by the user and include them in the documentation.

4. **Create a detailed Markdown file. The file name must end with the current date and time in the following format: `Solution-Overview-yyyy-MM-dd-hh_mm_ss.md`**
   - Where:
     - `yyyy` is the 4-digit year
     - `MM` is the 2-digit month
     - `dd` is the 2-digit day
     - `hh` is the 2-digit hour (24-hour format)
     - `mm` is the 2-digit minute
     - `ss` is the 2-digit second
   - For example: `Solution-Overview-2025-07-11-12_30_22.md`
   - Explains the goal and purpose of the solution.
   - Describes the architecture, main components, and their interactions.

   - **Includes the generated architecture diagram as a separate section in the documentation file, in the format selected by the user.**
   - Example ASCII diagram:

```
+-------------------+
|   AspireApp1      |
|    .AppHost       |
+-------------------+
         |
         | orchestrates
         v
+-------------------+      ++-------------------+
| AspireApp1.Web    |<---->| AspireApp1.ApiSvc |
| (Angular Frontend)|      | (Weather API)     |
+-------------------+      ++-------------------+
         |                        ^
         | uses                   |
         v                        |
+-------------------+             |
|   Redis Cache     |<-------------+
+-------------------+
```

   - Provides as much detail as possible about the solution’s design, features, and intended use.
   - **The generated documentation must be saved in a `docs` folder at the root of the repository. If the `docs` folder does not exist, it must be created.**
5. **(Optional, only if user confirms)**: Use Playwright MCP server tools to:
   - Capture a screenshot of the Aspire dashboard.
   - Capture a screenshot of the main frontend page.
   - If Playwright MCP server tools can't launch the app, ask the user for the correct launch URL.
   - **All screenshots must be saved in the `docs/screenshots` folder. If the `docs/screenshots` folder does not exist, it must be created.**

**Instructions:**
- Ensure the markdown is well-structured, with sections for Overview, Architecture, Components, and Screenshots.
- Use the screenshots to visually illustrate the dashboard and frontend.
- Be thorough in your analysis and explanations.