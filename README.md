# Azure DevOps Demo Generator

## About

The Azure DevOps Demo Generator can create projects in your Azure DevOps organization, prepopulated with template-based content including source code, work items, iterations, service endpoints, build and release definitions, and more!

The original purpose of this system is to simplify working with the [Azure DevOps hands-on-labs](https://www.azuredevopslabs.com), demos and other education material. But it can also be used to drive your ownAzure DevOps automation utilities, provision your own custom templates, or as a reference for using the [Azure DevOps REST APIs](https://learn.microsoft.com/en-us/rest/api/azure/devops/).

## Contributions

This project welcomes contributions and suggestions.  Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

For more information on contributing, please visit the [contributor guide](./CONTRIBUTING.md).

## Contributors:

- [Akshay Hosur](https://github.com/akshay-online)

# OneBit Customization:

Added CT-OneBitTemplate custom template. That's been generated from Forms Express eNotices Azure DevOps project, but extensive modifications have been applied to it.

Creating a project off the template will set up the following:

- New project in Azure DevOps off the Agile template.
- Project will be configured with the default project team and area. The current user will be assigned as a single team member.
- One-year worth of 2 week iterations, starting on the project creation date, will be set up and assigned to the default team.
- A repository will be initialized with dev and main branches and some sample structural folders. The dev branch will be default.
- Branch policies will be configured on both branches, defining PR merge policies, build validation, etc.
- Sample YAML build pipelines will be set up for building the dev branch (automatic trigger); building the main branch (automatic trigger); PR validation (manual trigger); generating release notes (manual trigger).
- Sample release pipelines will be set up for releasing to staging and production environments.
- Several common epics and features will be created in the project backlog.

Known remaining TODOs:

- Test the template creator outside of the Forms Express organisation in Azure DevOps.
- Initialize sample projects and test the pipelines.
- Document possible changes to template configuration files.
- Document remaining manual steps to have a fully functional Azure DevOps project, e.g., required permissions to make release notes publishing completely functional.
- Make YAML build pipelines more generic, removing remaining references to Forms Express eNotices solution.
- Initialize project Wiki with a release notes page, as the release notes pipeline depend on it.
- Clean up source code of tool as most of it is of quite poor quality.