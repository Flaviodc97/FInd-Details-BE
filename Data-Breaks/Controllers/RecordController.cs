using Data_Breaks.Class;
using Data_Breaks.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Cors;

namespace Data_Breaks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordController : Controller
    {
        private const string directoryPath = "indexFolder";
        private readonly ILogger<RecordController> _logger;
        private readonly List<Record> records;
        private readonly IRecordSearcherEngine _recordSearcherEngine;

        public RecordController(ILogger<RecordController> logger, IRecordSearcherEngine recordSearcherEngine)
        {
            _logger = logger;
            _recordSearcherEngine = recordSearcherEngine;
        }

        
        [Microsoft.AspNetCore.Mvc.HttpGet("Search")]
        public IActionResult Search(string name, string surname)
        {
            try 
            {
                List<Record> data = new List<Record>();
                if (Directory.Exists(directoryPath))
                {
                    string[] files = Directory.GetFiles(directoryPath);
                    string[] directories = Directory.GetDirectories(directoryPath);

                    if (files.Length == 0 && directories.Length == 0)
                    {
                        _recordSearcherEngine.CreateIndex();
                        data = _recordSearcherEngine.Search(name, surname);
                    }
                    else
                    {
                        data = _recordSearcherEngine.Search(name, surname);
                    }
                }
                else 
                {
                    Directory.CreateDirectory(directoryPath);
                    _recordSearcherEngine.CreateIndex();
                    data = _recordSearcherEngine.Search(name, surname);
                }

                    
                
                if(data == null || data.Count == 0) return NotFound();
                return Ok(data);
                
            }
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
            
            
        }
       
    }

   
}
