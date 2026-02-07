namespace UI
{
    public sealed record CategoryDto(Guid Id, string Name, List<BudgetDto> Budgets);
    public sealed record BudgetDto(int Year, int Month, decimal Amount);

    public sealed record ChatRequest(string Prompt, bool Confirm = false);
    public sealed record ChatResponse(string Message, bool NeedsConfirmation);
    public sealed record BudgetPivotRowDto(
    Guid CategoryId,
    string Category,
    decimal January,
    decimal February,
    decimal March,
    decimal April,
    decimal May,
    decimal June,
    decimal July,
    decimal August,
    decimal September,
    decimal October,
    decimal November,
    decimal December,
    decimal Total);

}
