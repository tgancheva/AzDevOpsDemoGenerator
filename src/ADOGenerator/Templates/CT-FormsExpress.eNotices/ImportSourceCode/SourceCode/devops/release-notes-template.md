# Release Notes - {{buildDetails.buildNumber}}
**Date & Time:** {{buildDetails.startTime}}

## Features
| **Id** | **Type** | **Title** |
|--|--|--|
{{#if this.workItems}}
  {{#forEach this.workItems}}
    {{#if (eq this.fields.[System.WorkItemType] 'User Story')}}
| {{this.id}} | {{this.fields.[System.WorkItemType]}} | [{{this.fields.[System.Title]}}]({{replace this.url "_apis/wit/workItems" "_workitems/edit"}}) |
    {{/if}}
  {{/forEach}}
  {{#forEach this.workItems}}
    {{#if (eq this.fields.[System.WorkItemType] 'Task')}}
      {{#unless (contains (pluck ../workItems 'id') this.fields.[System.Parent])}}
| {{this.id}} | {{this.fields.[System.WorkItemType]}} | [{{this.fields.[System.Title]}}]({{replace this.url "_apis/wit/workItems" "_workitems/edit"}}) |
      {{/unless}}
    {{/if}}
  {{/forEach}}
{{else}}
None
{{/if}}

## Bug fixes
| **Id** | **Type** | **Title** |
|--|--|--|
{{#if this.workItems}}
{{#forEach this.workItems}}
{{#if (eq this.fields.[System.WorkItemType] 'Bug')}}
| {{this.id}} | {{this.fields.[System.WorkItemType]}} | [{{this.fields.[System.Title]}}]({{replace this.url "_apis/wit/workItems" "_workitems/edit"}}) |
{{/if}}
{{/forEach}}
{{else}}
None
{{/if}}