SELECT tp.Id, tp.Name, tp.StartDate, tp.MaxAttendees
FROM TrainingProgram tp
LEFT JOIN EmployeeTraining et on et.TrainingProgramId = tp.Id
GROUP BY tp.Id, tp.Name, tp.StartDate, tp.MaxAttendees
HAVING tp.StartDate > GETDATE() AND tp.MaxAttendees > COUNT(et.TrainingProgramId)
UNION
SELECT tp.Id, tp.Name, tp.StartDate, tp.MaxAttendees
FROM TrainingProgram tp
LEFT JOIN EmployeeTraining et on et.TrainingProgramId = tp.Id
WHERE et.EmployeeId = 1 AND tp.StartDate > GETDATE()

--SELECT et.TrainingProgramId, COUNT(et.TrainingProgramId) 
--		FROM EmployeeTraining et
--		GROUP By et.TrainingProgramId

--SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, tp.Name, tp.Id AS TheTrainingId
--                                        FROM Employee e
--                                        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id 
--                                        LEFT JOIN TrainingProgram tp on e.id = tp.Id
--                                        WHERE e.Id = 1

SELECT et.EmployeeId, et.TrainingProgramId, et.Id, tp.Name 
			FROM EmployeeTraining et
			LEFT JOIN TrainingProgram tp on et.TrainingProgramId = tp.Id
			WHERE et.EmployeeId = 1

SELECT e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSupervisor, tp.Name, tp.Id AS TheTrainingId
                                        FROM Employee e
                                        LEFT JOIN EmployeeTraining et ON et.EmployeeId = e.Id 
                                        LEFT JOIN TrainingProgram tp on et.TrainingProgramId  = tp.Id
                                        WHERE e.Id = 1 AND tp.StartDate > GETDATE()