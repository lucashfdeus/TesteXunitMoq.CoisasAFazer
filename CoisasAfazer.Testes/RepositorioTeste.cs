using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoisasAfazer.Testes {

    class RepositorioTeste : IRepositorioTarefas {

        List<Tarefa> lista = new List<Tarefa>();

        public void IncluirTarefas( params Tarefa[] tarefas ) {
            throw new Exception( "Houve um erro ao incluir tarefas." );
            tarefas.ToList().ForEach(t => lista.Add(t));
        }

        public IEnumerable<Tarefa> ObtemTarefas( Func<Tarefa, bool> filtro ) {
            return lista.Where( filtro );
        }

        public Categoria ObtemCategoriaPorId( int id ) {
            throw new NotImplementedException();
        }

        public void AtualizarTarefas( params Tarefa[] tarefas ) {
            throw new NotImplementedException();
        }

        public void ExcluirTarefas( params Tarefa[] tarefas ) {
            throw new NotImplementedException();
        }


    }
}
