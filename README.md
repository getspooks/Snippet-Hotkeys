# Snippet-Hotkeys
A lightweight Windows desktop application that allows users to trigger reusable text snippets from global keyboard shortcuts.

Originally developed to reduce repetitive typing, improve consistency, and speed up common customer communications for dispatchers.

| Feature | Description |
|----------|-------------|
| Global Hotkeys | Trigger snippets from anywhere in Windows |
| Token Expansion | Insert dynamic dates and formatting tokens |
| Browser Compatible | Works in Gmail and other web editors |
| Hotkey Validation | Detect duplicate or invalid shortcuts |
| Token Reference | Built-in token documentation window |
| Portable Deployment | Single executable, no installation required |


<details>
<summary>Example Tokens</summary>

{TODAY}
{NOW}
{NEXT_BUSINESS_DAY}
{NEXT_BUSINESS_DAY_DATE}

## Example

Snippet:

```text
This has been scheduled for {NEXT_BUSINESS_DAY_DATE}.
```

Output:

```text
This has been scheduled for tomorrow 6/25.
```

</details>

## Typical Use Cases

- Dispatch scheduling updates
- Customer service responses
- Email templates
- Frequently used internal communications

## Technologies

| Component | Technology |
|------------|------------|
| Language | C# |
| Framework | .NET 8 |
| UI | WinForms |
| Input Handling | Windows SendInput API |
| Storage | JSON Configuration Files |

## Installation

1. Download the latest release.
2. Extract the ZIP file.
3. Run SnippetHotkeys.exe.
