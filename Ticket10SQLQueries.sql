SELECT tp.Id, tp.Name, tp.StartDate, tp.MaxAttendees
FROM TrainingProgram tp
LEFT JOIN EmployeeTraining et on et.TrainingProgramId = tp.Id
GROUP BY tp.Id, tp.Name, tp.StartDate, tp.MaxAttendees
HAVING tp.StartDate > GETDATE() AND tp.MaxAttendees > COUNT(et.TrainingProgramId)
UNION
SELECT tp.Id, tp.Name, tp.StartDate, tp.MaxAttendees
FROM TrainingProgram tp
LEFT JOIN EmployeeTraining et on et.TrainingProgramId = tp.Id
WHERE et.EmployeeId = 2 AND tp.StartDate > GETDATE()

SELECT et.TrainingProgramId, COUNT(et.TrainingProgramId) 
		FROM EmployeeTraining et
		GROUP By et.TrainingProgramId