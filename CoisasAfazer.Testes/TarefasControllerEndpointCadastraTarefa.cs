using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CoisasAfazer.Testes {

    public class TarefasControllerEndpointCadastraTarefa {

        [Fact]
        public void DataTarefaComInfoValidasDeveRetornar200() {

            //Arranger
           
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase( "DbTarefasContext" )
                .Options;
            var contexto = new DbTarefasContext( options );
            contexto.Categorias.Add(new Categoria(20, "Estudo"));
            contexto.SaveChanges();

            var repo = new RepositorioTarefa(contexto);
            var controlador = new TarefasController(repo, mockLogger.Object);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar Xunit";
            model.Prazo = new DateTime( 2019, 12, 31 );

            //Atc
            var retorno = controlador.EndpointCadastraTarefa(model);

            //Assert 
            Assert.IsType<OkResult>( retorno ); //OkResult = 200
        }

        [Fact]
        public void QuandoExecaoForLancadaDeveRetornarStatusCode500() {

            //Arranger

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup( r => r.ObtemCategoriaPorId( 20 ) )
                .Returns( new Categoria( 20, "Estudo" ) );

            mock.Setup( r =>
                 r.IncluirTarefas( It.IsAny<Tarefa[]>() ) )
                .Throws( new Exception( "Houve um erro" ) );
            var repo = mock.Object;
            var controlador = new TarefasController( repo, mockLogger.Object );

            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar Xunit";
            model.Prazo = new DateTime( 2019, 12, 31 );

            //Atc
            var retorno = controlador.EndpointCadastraTarefa( model );

            //Assert 
            Assert.IsType<StatusCodeResult>( retorno ); //BadRequestResult = 400
            var statusCodeRetornado = ( retorno as StatusCodeResult).StatusCode;
            Assert.Equal( 500, statusCodeRetornado );
        }
    }
}
