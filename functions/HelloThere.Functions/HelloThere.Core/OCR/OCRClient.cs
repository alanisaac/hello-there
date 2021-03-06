﻿using HelloThere.Core.OCR;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloThere.Functions.OCR
{
    public class OCRClient : IOCRClient
    {
        private readonly ComputerVisionClient _computerVisionClient;
        private readonly ILogger _logger;
        private const TextRecognitionMode _textRecognitionMode = TextRecognitionMode.Printed;
        private const int _numberOfCharsInOperationId = 36;

        public OCRClient(string endpoint, string subscriptionKey, ILogger logger)
        {
            _logger = logger;

            var computerVisionClient = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(subscriptionKey),
                new System.Net.Http.DelegatingHandler[] { });

            computerVisionClient.Endpoint = endpoint;

            _computerVisionClient = computerVisionClient;
        }
        
        public async Task<IList<string>> ExtractTextAsync(string imageUrl)
        {
            if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                _logger.LogError("Invalid imageUrl:\n{0} \n", imageUrl);
                return null;
            }

            // Start the async process to recognize the text
            RecognizeTextHeaders textHeaders = await _computerVisionClient.RecognizeTextAsync(imageUrl, _textRecognitionMode);

            return await GetTextAsync(textHeaders.OperationLocation);
        }

        private async Task<IList<string>> GetTextAsync(string operationLocation)
        {
            // Retrieve the URI where the recognized text will be
            // stored from the Operation-Location header
            string operationId = operationLocation.Substring(operationLocation.Length - _numberOfCharsInOperationId);

            _logger.LogDebug($"\nCalling {nameof(ComputerVisionClientExtensions.GetTextOperationResultAsync)}");
            TextOperationResult result = await _computerVisionClient.GetTextOperationResultAsync(operationId);

            // Wait for the operation to complete
            int i = 0;
            int maxRetries = 10;
            while ((result.Status == TextOperationStatusCodes.Running ||
                    result.Status == TextOperationStatusCodes.NotStarted) && i++ < maxRetries)
            {
                _logger.LogDebug($"Server status: {result.Status}, waiting {i} seconds...");
                await Task.Delay(1000);

                result = await _computerVisionClient.GetTextOperationResultAsync(operationId);
            }

            return result.RecognitionResult.Lines.Select(x => x.Text).ToList();
        }
    }
}
