﻿using System.Text.Json;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Planning;

using MSCoders.Demo.Services.VacationPlanner.Models;

namespace MSCoders.Demo.Services.VacationPlanner.Services;

internal sealed class VacationPlannerService
{
    private readonly Kernel kernel;
    private readonly ILogger logger;
    private readonly ISemanticTextMemory memory;

    public VacationPlannerService(Kernel kernel, ISemanticTextMemory memory, ILogger<VacationPlannerService> logger)
    {
        this.kernel = kernel;
        this.memory = memory;
        this.logger = logger;
    }

    public async Task<VacationPlannerResponse> ResponseAsync(VacationPlannerRequest request, CancellationToken cancellationToken)
    {
        var prompt = $@"
Provide an answer to the user’s inquiry in the same language that the user is using.
If the user provides only a month as date, consider the first and last day of the month as start and end date of the vacations respectively.
The inquiry from the user is: '{request.Ask}'

";

        var config = new FunctionCallingStepwisePlannerConfig
        {
            MaxIterations = 50,
            SemanticMemoryConfig = new SemanticMemoryConfig
            {
                Memory = memory,
            }
        };

        var planner = new FunctionCallingStepwisePlanner(config);

        string answer;

        try
        {
            var plannerResult = await planner.ExecuteAsync(kernel, prompt, cancellationToken);
            answer = plannerResult.FinalAnswer;

            logger.LogInformation($@"Planner history:\n{JsonSerializer.Serialize(plannerResult.ChatHistory)}");
        }
        catch (Exception exception)
        {
            answer = exception.Message;
        }

        return new VacationPlannerResponse
        {
            Answer = answer,
        };
    }
}
