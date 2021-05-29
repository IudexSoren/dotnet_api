using System;
using System.Collections.Generic;
using System.Linq;
using Commander.Models;

namespace Commander.Data
{
  public class SqlCommanderRepo : ICommanderRepo
  {
    private readonly CommanderContext _context;

    public SqlCommanderRepo(CommanderContext context)
    {
      _context = context;
    }

    // Agregar comando
    public void CreateCommand(Command cmd)
    {
      if (cmd == null)
      {
        throw new ArgumentNullException(nameof(cmd));
      }
      _context.Commands.Add(cmd);
    }

    // Eliminar comando
    public void DeleteCommand(Command cmd)
    {
      if (cmd == null)
      {
        throw new ArgumentNullException(nameof(cmd));
      }
      _context.Commands.Remove(cmd);
    }

    // Obtener la lista de comandos
    public IEnumerable<Command> GetAllCommands()
    {
      return _context.Commands.ToList();
    }

    // Obtener un comando en específico con el ID
    public Command GetCommandById(int id)
    {
      return _context.Commands.FirstOrDefault(p => p.Id == id);
    }

    // Guardar cambios en la base de datos
    public bool SaveChanges()
    {
      // Guardar los cambios después de realizada una acción en la base de datos
      return (_context.SaveChanges() >= 0);
    }

    // Actualizar comando
    public void UpdateCommand(Command cmd)
    {
      // Does nothing
    }
  }
}