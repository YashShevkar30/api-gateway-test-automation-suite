#!/usr/bin/env python3
import concurrent.futures
import dataclasses
import random
import time
from collections import Counter


@dataclasses.dataclass
class RequestResult:
    status_code: int
    latency_ms: float
    defect_category: str


def simulate_gateway_call() -> RequestResult:
    latency_ms = random.uniform(45, 600)
    if latency_ms > 450:
        return RequestResult(504, latency_ms, "UPSTREAM_TIMEOUT")
    if random.random() < 0.03:
        return RequestResult(429, latency_ms, "RATE_LIMIT")
    if random.random() < 0.02:
        return RequestResult(500, latency_ms, "INTERNAL_ERROR")
    return RequestResult(200, latency_ms, "NONE")


def run_load(total_requests: int = 1000, workers: int = 40) -> None:
    started = time.perf_counter()
    results: list[RequestResult] = []

    with concurrent.futures.ThreadPoolExecutor(max_workers=workers) as executor:
        futures = [executor.submit(simulate_gateway_call) for _ in range(total_requests)]
        for future in concurrent.futures.as_completed(futures):
            results.append(future.result())

    defect_breakdown = Counter(result.defect_category for result in results if result.defect_category != "NONE")
    p95_latency = sorted(result.latency_ms for result in results)[int(0.95 * len(results)) - 1]
    success_rate = (sum(result.status_code == 200 for result in results) / len(results)) * 100
    elapsed = time.perf_counter() - started

    print(f"Requests: {len(results)}")
    print(f"Success rate: {success_rate:.2f}%")
    print(f"P95 latency: {p95_latency:.1f} ms")
    print(f"Elapsed: {elapsed:.2f} sec")
    print("Defect categories:", dict(defect_breakdown))


if __name__ == "__main__":
    run_load()
