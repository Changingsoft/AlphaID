package com.tobaccoid.samples.resourceserver.controller;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("api/echo")
public class EchoController {
    @GetMapping()
    public String Echo(String input) {
        return input;
    }
}
