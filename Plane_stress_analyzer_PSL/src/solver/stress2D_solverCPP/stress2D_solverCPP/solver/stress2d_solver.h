#pragma once

#include "../system_store/stopwatch_events.h"
#include "../system_store/stress_system_store.h"
#include "../solver/polynomial_2dmesh_store.h"

class stress2d_solver
{
public:
	stress2d_solver();
	~stress2d_solver() = default;

	bool initialize_solver(stress_system_store* stress_system, int polynomial_order, stopwatch_events* stopwatch,
		void(*callback)(const char*));

	void perform_solve();


private:
	int polynomial_order = 0;
	polynomial_2dmesh_store polynomial_2dmesh;
	stopwatch_events* m_stopwatch;

	void(*m_callback)(const char*) = nullptr;

	void report(const char* msg);




};