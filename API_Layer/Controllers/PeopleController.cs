using BusinessLogicLayer.Interfaces;
using Contracts.DTOs.PersonDTOs;
using Microsoft.AspNetCore.Mvc;

namespace API_Layer.Controllers
{
    [Route("api/People")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly IPersonService _personService;
        public PeopleController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet("Get/{id}", Name = "GetPersonByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PersonResponseDTO>> GetPersonByID(int id)
        {
            if (id < 1)
                return BadRequest($"This ID Isn't Valid {id}");

            var PersonDTO = await _personService.GetPersonByIDAsync(id);

            if (PersonDTO == null)
                return NotFound($"Person With This ID = {id} Not Found");

            return Ok(PersonDTO);
        }

        [HttpPut("Update", Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdatePerson(UpdatePersonDTO updatedperson)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var originalPerson = await _personService.GetPersonByIDAsync(updatedperson.PersonID);
            if (originalPerson == null)
                return NotFound("Person With This ID Not Found");

            if (!await _personService.UpdatePersonAsync(updatedperson))
                return StatusCode(500, "Internal Server Error While Updating");

            return Ok(updatedperson);
        }

        [HttpPost("AddNew", Name = "AddNewPerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreatePersonDTO>> AddNewPerson(CreatePersonDTO person)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int? newPersonID = await _personService.AddNewPersonAsync(person);

            if (newPersonID == null)
                return StatusCode(500, "Internal server Error While Adding");

            return CreatedAtRoute("GetPersonByID", new { id = newPersonID }, person);
        }

        [HttpGet("All", Name = "GetAllPeople")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PersonResponseDTO>>> GetAllPeople()
        {
            var people = await _personService.GetAllPeopleAsync();

            return Ok(people);
        }

        [HttpDelete("Delete", Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeletePerson(int id)
        {
            if (id < 1)
                return BadRequest($"Invalid ID");

            if (await _personService.DeletePersonByIDAsync(id))
                return NoContent();
            else
                return NotFound($"Person With This ID Not Found {id}");
        }
    }
}