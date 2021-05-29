using System.Collections.Generic;
using AutoMapper;
using Commander.Data;
using Commander.DTOs;
using Commander.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Commander.Controllers
{
  // api/commands
  [Route("api/commands")]
  [ApiController]
  public class CommandsController : ControllerBase
  {
    private readonly ICommanderRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommanderRepo repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    // api/commands
    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDTO>> GetAllCommands()
    {
      var commandItems = _repository.GetAllCommands();

      // Mapeo definido en ../Profiles/CommandsProfile.cs
      return Ok(_mapper.Map<IEnumerable<CommandReadDTO>>(commandItems));
    }

    // api/commands/{ id }
    [HttpGet("{id}", Name = "GetCommandById")]
    public ActionResult<CommandReadDTO> GetCommandById(int id)
    {
      var commandItem = _repository.GetCommandById(id);
      if (commandItem == null)
      {
        return NotFound();
      }

      // CommandReadDTO retorna toda la información del comando sin el atributo Platform
      // Mapeo definido en ../Profiles/CommandsProfile.cs
      return Ok(_mapper.Map<CommandReadDTO>(commandItem));
    }

    // api/commands
    [HttpPost]
    public ActionResult<CommandReadDTO> CreateCommand(CommandCreateDTO commandCreateDTO)
    {
      var commandModel = _mapper.Map<Command>(commandCreateDTO);
      _repository.CreateCommand(commandModel);
      _repository.SaveChanges();

      var commandReadDTO = _mapper.Map<CommandReadDTO>(commandModel);

      // Retornar el nuevo comando con la dirección (Location) en la que fue almacenado el
      // nuevo comando y el status de la petición (201)
      return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDTO.Id }, commandReadDTO);
    }

    // api/commands/{ id }
    [HttpPut("{id}")]
    public ActionResult UpdateCommand(int id, CommandUpdateDTO commandUpdateDTO)
    {
      var commandModelRepo = _repository.GetCommandById(id);
      if (commandModelRepo == null)
      {
        return NotFound();
      }

      _mapper.Map(commandUpdateDTO, commandModelRepo);
      _repository.UpdateCommand(commandModelRepo);
      _repository.SaveChanges();

      return NoContent();
    }

    // api/commands/{ id }
    // Este método aplica los cambios únicamente a lo que fue modificado y no a todo el objeto
    /*
      Objeto necesario para hacer el cambio
        Path: Atributo que será modificado
        Value: Nuevo valor

      Para cambiar otro atributo se debe agregar otro objeto al array
      [
        {
          "op": "replace",
          "path": "/howto",
          "value": "Some new value"
        }
      ]
    */
    [HttpPatch("{id}")]
    public ActionResult PartialUpdateCommand(int id, JsonPatchDocument<CommandUpdateDTO> patchDoc)
    {
      var commandModelRepo = _repository.GetCommandById(id);
      if (commandModelRepo == null)
      {
        return NotFound();
      }

      // Command -> CommandUpdateDTO
      var commandToPatch = _mapper.Map<CommandUpdateDTO>(commandModelRepo);
      patchDoc.ApplyTo(commandToPatch, ModelState);
      if (!TryValidateModel(commandToPatch))
      {
        return ValidationProblem(ModelState);
      }

      _mapper.Map(commandToPatch, commandModelRepo);
      _repository.UpdateCommand(commandModelRepo);
      _repository.SaveChanges();

      return NoContent();
    }

    // api/commands/{ id }
    [HttpDelete("{id}")]
    public ActionResult DeleteCommand(int id)
    {
      var commandModelRepo = _repository.GetCommandById(id);
      if (commandModelRepo == null)
      {
        return NotFound();
      }

      _repository.DeleteCommand(commandModelRepo);
      _repository.SaveChanges();

      return NoContent();
    }
  }
}